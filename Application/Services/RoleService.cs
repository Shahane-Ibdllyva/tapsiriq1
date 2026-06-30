using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper; // Əlavə edildi
using FluentValidation; // Əlavə edildi
using Application.DTOs.Role;
using Application.Exceptions;
using Domain.Models;
using Application.Repositories;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IValidator<CreateRoleDto> _createValidator;
        private readonly IValidator<UpdateRoleDto> _updateValidator;
        private readonly IMapper _mapper;

        public RoleService(
            IRoleRepository roleRepository,
            IValidator<CreateRoleDto> createValidator,
            IValidator<UpdateRoleDto> updateValidator,
            IMapper mapper)
        {
            _roleRepository = roleRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleListDto>> GetAllRolesAsync()
        {
            return await _roleRepository.GetActiveRolesDtoAsync();
        }

        public async Task<RoleListDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetRoleWithDetailsAsync(id);

            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan rol sistemdə tapılmadı!");

            return _mapper.Map<RoleListDto>(role);
        }

        public async Task CreateRoleAsync(CreateRoleDto dto)
        {
            // 1. Formal Validasiya
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. Biznes Məntiqi yoxlaması
            var roleExists = await _roleRepository.CheckDuplicateAsync(dto.Name);
            if (roleExists)
                throw new ConflictException($"Sistemdə '{dto.Name}' adlı rol artıq mövcuddur!");

            // 3. AutoMapper Tətbiqi
            var role = _mapper.Map<Role>(dto);
            role.Status = EntityStatus.Active;

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task UpdateRoleAsync(UpdateRoleDto dto)
        {
            // 1. Formal Validasiya
            await _updateValidator.ValidateAndThrowAsync(dto);

            // 2. Rolun Mövcudluğu
            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.RoleId} ID-li rol tapılmadı!");

            // 3. Duplikat Yoxlaması
            var nameExists = await _roleRepository.CheckDuplicateForUpdateAsync(dto.Name, dto.RoleId);
            if (nameExists)
                throw new ConflictException($"Sistemdə '{dto.Name}' adlı rol artıq istifadə olunub!");

            // 4. AutoMapper ilə Mövcud Obyektin Yenilənməsi
            _mapper.Map(dto, role);
            role.UpdateDate = DateTime.UtcNow;

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null || role.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li rol tapılmadı!");

            role.Status = EntityStatus.Deleted;
            role.UpdateDate = DateTime.UtcNow;

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }
    }
}