using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Application.DTOs.Organization;
using Domain.Models;
using Application.Interfaces;          
using Application.Services.Interfaces;
using FluentValidation;
using Application.Exceptions;

namespace Application.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IUnitOfWork _unitOfWork;      
        private readonly IValidator<CreateOrganizationDto> _createOrganizationValidator;
        private readonly IValidator<UpdateOrganizationDto> _updateOrganizationValidator;
        private readonly IMapper _mapper;

        public OrganizationService(
            IUnitOfWork unitOfWork,                    
            IValidator<CreateOrganizationDto> createOrganizationValidator,
            IValidator<UpdateOrganizationDto> updateOrganizationValidator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createOrganizationValidator = createOrganizationValidator;
            _updateOrganizationValidator = updateOrganizationValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync()
        {
            var organizations = await _unitOfWork.Organizations.GetOrganizationsWithDepartmentsAsync();
            return _mapper.Map<IEnumerable<OrganizationDto>>(organizations);
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(int id)
        {
            var organization = await _unitOfWork.Organizations.GetOrganizationByIdWithDepartmentsAsync(id);

            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan təşkilat sistemdə tapılmadı!");

            return _mapper.Map<OrganizationDto>(organization);
        }

        public async Task CreateOrganizationAsync(CreateOrganizationDto dto)
        {
            await _createOrganizationValidator.ValidateAndThrowAsync(dto);

            var exists = await _unitOfWork.Organizations.CheckDuplicateNameAsync(dto.OrganizationName);
            if (exists)
                throw new ConflictException("Bu adda təşkilat artıq sistemdə mövcuddur!");

            var organization = _mapper.Map<Organization>(dto);
            organization.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Organizations.AddAsync(organization);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateOrganizationAsync(UpdateOrganizationDto dto)
        {
            await _updateOrganizationValidator.ValidateAndThrowAsync(dto);

            var organization = await _unitOfWork.Organizations.GetByIdAsync(dto.OrganizationId);
            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.OrganizationId} ID-li təşkilat tapılmadı!");

            var nameExists = await _unitOfWork.Organizations.CheckDuplicateNameForUpdateAsync(
                dto.OrganizationName,
                dto.OrganizationId);
            if (nameExists)
                throw new ConflictException($"'{dto.OrganizationName}' adlı təşkilat artıq mövcuddur!");

            _mapper.Map(dto, organization);
            organization.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Organizations.Update(organization);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteOrganizationAsync(int id)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(id);
            if (organization == null || organization.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li təşkilat tapılmadı!");

            organization.Status = EntityStatus.Deleted;
            organization.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Organizations.Update(organization);
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