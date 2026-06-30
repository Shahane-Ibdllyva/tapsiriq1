using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.UserRole;
using Application.Exceptions;
using Domain.Models;
using Application.Repositories;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateUserRoleDto> _createValidator;
        private readonly IValidator<UpdateUserRoleDto> _updateValidator;

        public UserRoleService(
            IUserRoleRepository userRoleRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IMapper mapper,
            IValidator<CreateUserRoleDto> createValidator,
            IValidator<UpdateUserRoleDto> updateValidator)
        {
            _userRoleRepository = userRoleRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<UserRoleListDto>> GetRolesByUserIdAsync(int userId)
        {
            var userExists = await _userRepository.CheckActiveUserExistsAsync(userId);
            if (!userExists)
                throw new NotFoundException($"ID-si {userId} olan istifadəçi tapılmadı!");

            return await _userRoleRepository.GetUserRolesDtoByUserIdAsync(userId);
        }

        public async Task AssignRoleToUserAsync(CreateUserRoleDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var userExists = await _userRepository.CheckActiveUserExistsAsync(dto.UserId);
            if (!userExists)
                throw new NotFoundException($"İstifadəçi (ID: {dto.UserId}) tapılmadı!");

            var roleExists = await _roleRepository.CheckActiveRoleExistsAsync(dto.RoleId);
            if (!roleExists)
                throw new NotFoundException($"Rol (ID: {dto.RoleId}) tapılmadı!");

            var existingUserRole = await _userRoleRepository.GetByUserIdAndRoleIdAsync(dto.UserId, dto.RoleId);

            if (existingUserRole != null)
            {
                if (existingUserRole.Status == EntityStatus.Deleted)
                {
                    existingUserRole.Status = EntityStatus.Active;
                    existingUserRole.UpdateDate = DateTime.UtcNow;
                    _userRoleRepository.Update(existingUserRole);
                    await _userRoleRepository.SaveChangesAsync();
                    return;
                }

                throw new ConflictException("Bu istifadəçi qeyd olunan rola artıq sahibdir!");
            }

            var userRole = _mapper.Map<UserRole>(dto);
            userRole.Status = EntityStatus.Active;

            await _userRoleRepository.AddAsync(userRole);
            await _userRoleRepository.SaveChangesAsync();
        }

        public async Task UpdateUserRoleAsync(UpdateUserRoleDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var userRole = await _userRoleRepository.GetByIdAsync(dto.UserRoleId);
            if (userRole == null || userRole.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.UserRoleId} ID-li User-Role əlaqəsi tapılmadı!");

            var userExists = await _userRepository.CheckActiveUserExistsAsync(dto.UserId);
            if (!userExists)
                throw new NotFoundException($"İstifadəçi (ID: {dto.UserId}) tapılmadı!");

            var roleExists = await _roleRepository.CheckActiveRoleExistsAsync(dto.RoleId);
            if (!roleExists)
                throw new NotFoundException($"Rol (ID: {dto.RoleId}) tapılmadı!");

            var duplicateExists = await _userRoleRepository.CheckDuplicateForUpdateAsync(dto.UserId, dto.RoleId, dto.UserRoleId);
            if (duplicateExists)
                throw new ConflictException("Bu istifadəçi və rol kombinasiyası digər sətirdə artıq aktivdir!");

            _mapper.Map(dto, userRole);
            userRole.UpdateDate = DateTime.UtcNow;

            _userRoleRepository.Update(userRole);
            await _userRoleRepository.SaveChangesAsync();
        }

        public async Task RemoveRoleFromUserAsync(int id)
        {
            var userRole = await _userRoleRepository.GetByIdAsync(id);
            if (userRole == null || userRole.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li User-Role əlaqəsi tapılmadı!");

            userRole.Status = EntityStatus.Deleted;
            userRole.UpdateDate = DateTime.UtcNow;

            _userRoleRepository.Update(userRole);
            await _userRoleRepository.SaveChangesAsync();
        }
    }
}