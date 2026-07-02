using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.Role;
using Application.Exceptions;
using Domain.Models;
using Application.Interfaces;          
using Application.Services.Interfaces;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;      
        private readonly IValidator<CreateRoleDto> _createValidator;
        private readonly IValidator<UpdateRoleDto> _updateValidator;
        private readonly IMapper _mapper;

        public RoleService(
            IUnitOfWork unitOfWork,                   
            IValidator<CreateRoleDto> createValidator,
            IValidator<UpdateRoleDto> updateValidator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleListDto>> GetAllRolesAsync()
        {
            return await _unitOfWork.Roles.GetActiveRolesDtoAsync();
        }

        public async Task<RoleListDto?> GetRoleByIdAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetRoleWithDetailsAsync(id);
            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan rol sistemdə tapılmadı!");
            return _mapper.Map<RoleListDto>(role);
        }

        public async Task CreateRoleAsync(CreateRoleDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var roleExists = await _unitOfWork.Roles.CheckDuplicateAsync(dto.Name);
            if (roleExists)
                throw new ConflictException($"Sistemdə '{dto.Name}' adlı rol artıq mövcuddur!");

            var role = _mapper.Map<Role>(dto);
            role.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Roles.AddAsync(role);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateRoleAsync(UpdateRoleDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var role = await _unitOfWork.Roles.GetByIdAsync(dto.RoleId);
            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.RoleId} ID-li rol tapılmadı!");

            var nameExists = await _unitOfWork.Roles.CheckDuplicateForUpdateAsync(dto.Name, dto.RoleId);
            if (nameExists)
                throw new ConflictException($"Sistemdə '{dto.Name}' adlı rol artıq istifadə olunub!");

            _mapper.Map(dto, role);
            role.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Roles.Update(role);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(id);
            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li rol tapılmadı!");

            role.Status = EntityStatus.Deleted;
            role.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Roles.Update(role);
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