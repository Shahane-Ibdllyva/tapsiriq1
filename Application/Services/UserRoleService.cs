using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.UserRole;
using Application.Exceptions;
using Domain.Models;
using Application.Interfaces;          // <-- IUnitOfWork üçün
using Application.Services.Interfaces;

namespace Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUnitOfWork _unitOfWork;      // <-- UnitOfWork
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserRoleDto> _createValidator;
        private readonly IValidator<UpdateUserRoleDto> _updateValidator;

        public UserRoleService(
            IUnitOfWork unitOfWork,                    // <-- UnitOfWork
            IMapper mapper,
            IValidator<CreateUserRoleDto> createValidator,
            IValidator<UpdateUserRoleDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<UserRoleListDto>> GetRolesByUserIdAsync(int userId)
        {
            var userExists = await _unitOfWork.Users.CheckActiveUserExistsAsync(userId);
            if (!userExists)
                throw new NotFoundException($"ID-si {userId} olan istifadəçi tapılmadı!");

            return await _unitOfWork.UserRoles.GetUserRolesDtoByUserIdAsync(userId);
        }

        public async Task AssignRoleToUserAsync(CreateUserRoleDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var userExists = await _unitOfWork.Users.CheckActiveUserExistsAsync(dto.UserId);
            if (!userExists)
                throw new NotFoundException($"İstifadəçi (ID: {dto.UserId}) tapılmadı!");

            var roleExists = await _unitOfWork.Roles.CheckActiveRoleExistsAsync(dto.RoleId);
            if (!roleExists)
                throw new NotFoundException($"Rol (ID: {dto.RoleId}) tapılmadı!");

            var existingUserRole = await _unitOfWork.UserRoles.GetByUserIdAndRoleIdAsync(dto.UserId, dto.RoleId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (existingUserRole != null)
                {
                    if (existingUserRole.Status == EntityStatus.Deleted)
                    {
                        existingUserRole.Status = EntityStatus.Active;
                        existingUserRole.UpdateDate = DateTime.UtcNow;
                        _unitOfWork.UserRoles.Update(existingUserRole);
                        await _unitOfWork.SaveChangesAsync();
                        await _unitOfWork.CommitTransactionAsync();
                        return;
                    }
                    throw new ConflictException("Bu istifadəçi qeyd olunan rola artıq sahibdir!");
                }

                var userRole = _mapper.Map<UserRole>(dto);
                userRole.Status = EntityStatus.Active;

                await _unitOfWork.UserRoles.AddAsync(userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var userRole = await _unitOfWork.UserRoles.GetByIdAsync(dto.UserRoleId);
            if (userRole == null || userRole.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.UserRoleId} ID-li User-Role əlaqəsi tapılmadı!");

            var userExists = await _unitOfWork.Users.CheckActiveUserExistsAsync(dto.UserId);
            if (!userExists)
                throw new NotFoundException($"İstifadəçi (ID: {dto.UserId}) tapılmadı!");

            var roleExists = await _unitOfWork.Roles.CheckActiveRoleExistsAsync(dto.RoleId);
            if (!roleExists)
                throw new NotFoundException($"Rol (ID: {dto.RoleId}) tapılmadı!");

            var duplicateExists = await _unitOfWork.UserRoles.CheckDuplicateForUpdateAsync(
                dto.UserId, dto.RoleId, dto.UserRoleId);
            if (duplicateExists)
                throw new ConflictException("Bu istifadəçi və rol kombinasiyası digər sətirdə artıq aktivdir!");

            _mapper.Map(dto, userRole);
            userRole.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.UserRoles.Update(userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task RemoveRoleFromUserAsync(int id)
        {
            var userRole = await _unitOfWork.UserRoles.GetByIdAsync(id);
            if (userRole == null || userRole.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li User-Role əlaqəsi tapılmadı!");

            userRole.Status = EntityStatus.Deleted;
            userRole.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.UserRoles.Update(userRole);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}