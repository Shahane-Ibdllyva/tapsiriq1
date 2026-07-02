using Application.DTOs.Department;
using Application.Exceptions;
using Application.Interfaces;           // <-- IUnitOfWork üçün
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Models;
using FluentValidation;

namespace Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;     
        private readonly IValidator<CreateDepartmentDto> _createValidator;
        private readonly IValidator<UpdateDepartmentDto> _updateValidator;
        private readonly IMapper _mapper;

        public DepartmentService(
            IUnitOfWork unitOfWork,                    
            IValidator<CreateDepartmentDto> createValidator,
            IValidator<UpdateDepartmentDto> updateValidator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartmentListDto>> GetAllDepartmentsAsync()
        {
            return await _unitOfWork.Departments.GetActiveDepartmentsDtoAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetDepartmentWithDetailsAsync(id);

            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan şöbə sistemdə tapılmadı!");

            return department;
        }

        public async Task CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            // Organization mövcudluğu (əgər varsa)
            var orgExists = await _unitOfWork.Organizations.ExistsAsync(dto.OrganizationId);
            if (!orgExists) throw new NotFoundException($"Təşkilat (ID: {dto.OrganizationId}) sistemdə tapılmadı!");

            var department = _mapper.Map<Department>(dto);
            department.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Departments.AddAsync(department);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateDepartmentAsync(UpdateDepartmentDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var department = await _unitOfWork.Departments.GetByIdAsync(dto.DepartmentId);
            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.DepartmentId} ID-li şöbə tapılmadı!");

            // Organization mövcudluğu (əgər varsa)
            var orgExists = await _unitOfWork.Organizations.ExistsAsync(dto.OrganizationId);
            if (!orgExists) throw new NotFoundException($"Təşkilat (ID: {dto.OrganizationId}) sistemdə tapılmadı!");

            dto.DepartmentName = dto.DepartmentName?.Trim();

            var isDuplicate = await _unitOfWork.Departments.CheckDuplicateForUpdateAsync(
                dto.OrganizationId,
                dto.DepartmentName,
                dto.DepartmentId);

            if (isDuplicate)
                throw new ConflictException($"Bu təşkilatda '{dto.DepartmentName}' adlı şöbə artıq istifadə olunub!");

            _mapper.Map(dto, department);
            department.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Departments.Update(department);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteDepartmentAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null || department.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li şöbə tapılmadı!");

            department.Status = EntityStatus.Deleted;
            department.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Departments.Update(department);
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