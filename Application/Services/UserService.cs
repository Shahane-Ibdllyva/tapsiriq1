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
using Application.Repositories;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserDto> _createValidator;
        private readonly IValidator<UpdateUserDto> _updateValidator;

        public UserService(
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IMapper mapper,
            IValidator<CreateUserDto> createValidator,
            IValidator<UpdateUserDto> updateValidator)
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllActiveUsersDtoAsync();
        }

        public async Task<UserListDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan istifadəçi sistemdə tapılmadı!");

            return _mapper.Map<UserListDto>(user);
        }

        public async Task CreateUserAsync(CreateUserDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var departmentExists = await _departmentRepository.CheckDepartmentExistsAsync(dto.DepartmentId);
            if (!departmentExists)
                throw new NotFoundException($"Daxil edilən Şöbə (ID: {dto.DepartmentId}) sistemdə tapılmadı!");

            var emailExists = await _userRepository.CheckEmailExistsAsync(dto.Email);
            if (emailExists)
                throw new ConflictException("Bu e-poçt ünvanı ilə qeydiyyatdan keçmiş istifadəçi artıq mövcuddur!");

            var user = _mapper.Map<User>(dto);
            user.PasswordHash = ComputeMD5Hash(dto.Password);
            user.Status = EntityStatus.Active;

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(UpdateUserDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {dto.UserId} olan istifadəçi sistemdə tapılmadı!");

            var departmentExists = await _departmentRepository.CheckDepartmentExistsAsync(dto.DepartmentId);
            if (!departmentExists)
                throw new NotFoundException($"Daxil edilən Şöbə (ID: {dto.DepartmentId}) tapılmadı!");

            var emailExists = await _userRepository.CheckEmailExistsForUpdateAsync(dto.Email, dto.UserId);
            if (emailExists)
                throw new ConflictException("Bu e-poçt ünvanı başqa bir istifadəçi tərəfindən istifadə edilir!");

            _mapper.Map(dto, user);
            user.UpdateDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li istifadəçi tapılmadı!");

            user.Status = EntityStatus.Deleted;
            user.UpdateDate = DateTime.UtcNow;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
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