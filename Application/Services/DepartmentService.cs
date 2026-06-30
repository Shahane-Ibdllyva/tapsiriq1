using Application.DTOs.Department;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IValidator<CreateDepartmentDto> _createValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateValidator;
        private readonly IMapper _mapper;

        public DepartmentService(
            IDepartmentRepository departmentRepository,
            IOrganizationRepository organizationRepository,
            IValidator<CreateDepartmentDto> createValidator,
            IValidator<UpdateDepartmentDto> updateValidator,
            IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _organizationRepository = organizationRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetActiveDepartmentsDtoAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentRepository.GetDepartmentWithDetailsAsync(id);

            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan şöbə sistemdə tapılmadı!");

            return department;
        }

        public async Task CreateDepartmentAsync(CreateDepartmentDto dto)
        {
          
            await _createValidator.ValidateAndThrowAsync(dto);

            var department = _mapper.Map<Department>(dto);

            await _departmentRepository.AddAsync(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task UpdateDepartmentAsync(UpdateDepartmentDto dto)
        {
          
        
            await _updateValidator.ValidateAndThrowAsync(dto);

          
            var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.DepartmentId} ID-li şöbə tapılmadı!");

            dto.DepartmentName = dto.DepartmentName?.Trim(); 
        
            var isDuplicate = await _departmentRepository.CheckDuplicateForUpdateAsync(
                dto.OrganizationId,
                dto.DepartmentName,
                dto.DepartmentId);

            if (isDuplicate)
                throw new ConflictException($"Bu təşkilatda '{dto.DepartmentName}' adlı şöbə artıq istifadə olunub!");

            _mapper.Map(dto, department);
            department.UpdateDate = DateTime.UtcNow;

            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li şöbə tapılmadı!");

            department.Status = EntityStatus.Deleted;
            _departmentRepository.Update(department);
            await _departmentRepository.SaveChangesAsync();
        }
    }
}