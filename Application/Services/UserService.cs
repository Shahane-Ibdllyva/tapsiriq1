using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.User;
using Application.Exceptions;
using Domain.Models;
using Application.Services.Interfaces;
using Application.Interfaces;
using tapsiriq1.Domain.Entities;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;      // <-- UnitOfWork
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;
        private readonly IAppFileService _fileService;

        public UserService(
            IUnitOfWork unitOfWork,                    // <-- UnitOfWork
            IMapper mapper,
            IValidator<CreateUserDto> createValidator,
            IValidator<UpdateUserDto> updateValidator,
            IAppFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _fileService = fileService;
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
        {
            return await _unitOfWork.Users.GetAllActiveUsersDtoAsync();
        }

        public async Task<UserListDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan istifadəçi sistemdə tapılmadı!");
            return _mapper.Map<UserListDto>(user);
        }

        public async Task CreateUserAsync(CreateUserDto dto, int currentUserId)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            // Department mövcudluğu - İndi UnitOfWork üzərindən!
            var departmentExists = await _unitOfWork.Departments.CheckDepartmentExistsAsync(dto.DepartmentId);
            if (!departmentExists)
                throw new NotFoundException($"Daxil edilən Şöbə (ID: {dto.DepartmentId}) sistemdə tapılmadı!");

            var emailExists = await _unitOfWork.Users.CheckEmailExistsAsync(dto.Email);
            if (emailExists)
                throw new ConflictException("Bu e-poçt ünvanı ilə qeydiyyatdan keçmiş istifadəçi artıq mövcuddur!");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = ComputeMD5Hash(dto.Password);
            user.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Users.AddAsync(user);

                if (dto.ProfilePicture != null)
                {
                    user.AppFile = await _fileService.ProcessAndCreateAsync(dto.ProfilePicture, currentUserId);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateUserAsync(UpdateUserDto dto, int currentUserId)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {dto.UserId} olan istifadəçi sistemdə tapılmadı!");

            // Department mövcudluğu - İndi UnitOfWork üzərindən!
            var departmentExists = await _unitOfWork.Departments.CheckDepartmentExistsAsync(dto.DepartmentId);
            if (!departmentExists)
                throw new NotFoundException($"Daxil edilən Şöbə (ID: {dto.DepartmentId}) tapılmadı!");

            var emailExists = await _unitOfWork.Users.CheckEmailExistsForUpdateAsync(dto.Email, dto.UserId);
            if (emailExists)
                throw new ConflictException("Bu e-poçt ünvanı başqa bir istifadəçi tərəfindən istifadə edilir!");

            _mapper.Map(dto, user);
            user.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (dto.ProfilePicture != null)
                {
                    if (user.ImageId.HasValue)
                    {
                        await _fileService.DeleteFileAsync(user.ImageId.Value, currentUserId);
                    }
                    user.AppFile = await _fileService.ProcessAndCreateAsync(dto.ProfilePicture, currentUserId);
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li istifadəçi tapılmadı!");

            user.Status = EntityStatus.Deleted;
            user.UpdateDate = DateTime.UtcNow;

            if (user.ImageId.HasValue)
            {
                await _fileService.DeleteFileAsync(user.ImageId.Value, 0);
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private string ComputeMD5Hash(string password)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}