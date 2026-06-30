using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper; 
using Application.DTOs.Organization;
using Domain.Models;
using Application.Repositories;
using Application.Services.Interfaces;
using FluentValidation;
using Application.Exceptions;

namespace Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IValidator<CreateOrganizationDto> _createOrganizationValidator;
        private readonly IValidator<UpdateOrganizationDto> _updateOrganizationValidator; 
        private readonly IMapper _mapper; 

        public OrganizationService(
            IOrganizationRepository organizationRepository,
            IValidator<CreateOrganizationDto> createOrganizationValidator,
            IValidator<UpdateOrganizationDto> updateOrganizationValidator,
            IMapper mapper)
        {
            _organizationRepository = organizationRepository;
            _createOrganizationValidator = createOrganizationValidator;
            _updateOrganizationValidator = updateOrganizationValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync()
        {
            var organizations = await _organizationRepository.GetOrganizationsWithDepartmentsAsync();
            return _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(int id)
        {
            var organization = await _organizationRepository.GetOrganizationByIdWithDepartmentsAsync(id);

            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan təşkilat sistemdə tapılmadı!");

            return _mapper.Map<OrganizationDto>(organization);
        }

        public async Task CreateOrganizationAsync(CreateOrganizationDto dto)
        {
            // 1. Formal validasiya
            await _createOrganizationValidator.ValidateAndThrowAsync(dto);

            // 2. Biznes məntiqi – təkrarlanan ad yoxlanışı
            var exists = await _organizationRepository.CheckDuplicateNameAsync(dto.OrganizationName);
            if (exists)
                throw new ConflictException("Bu adda təşkilat artıq sistemdə mövcuddur!");

            // 3. AutoMapper tətbiqi
            var organization = _mapper.Map<Organization>(dto);

            await _organizationRepository.AddAsync(organization);
            await _organizationRepository.SaveChangesAsync();
        }

        public async Task UpdateOrganizationAsync(UpdateOrganizationDto dto)
        {
            // 1. Formal validasiya (UpdateValidator ilə)
            await _updateOrganizationValidator.ValidateAndThrowAsync(dto);

            // 2. Təşkilatın mövcudluğu yoxlanılır
            var organization = await _organizationRepository.GetByIdAsync(dto.OrganizationId);
            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.OrganizationId} ID-li təşkilat tapılmadı!");

            // 3. Ad təkrarlanması yoxlaması (özündən başqa)
            var nameExists = await _organizationRepository.CheckDuplicateNameForUpdateAsync(dto.OrganizationName, dto.OrganizationId);
            if (nameExists)
                throw new ConflictException($"'{dto.OrganizationName}' adlı təşkilat artıq mövcuddur!");

            // 4. AutoMapper ilə mövcud obyektin üzərinə yazma (Map onto existing object)
            _mapper.Map(dto, organization);
            organization.UpdateDate = DateTime.UtcNow; // Audit üçün update tarixi

            _organizationRepository.Update(organization);
            await _organizationRepository.SaveChangesAsync();
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            var organization = await _organizationRepository.GetByIdAsync(id);
            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li təşkilat tapılmadı!");

            // Soft delete tətbiq olunursa:
            organization.Status = EntityStatus.Deleted;
            organization.UpdateDate = DateTime.UtcNow;

            _organizationRepository.Update(organization);
            await _organizationRepository.SaveChangesAsync();
        }
    }
}