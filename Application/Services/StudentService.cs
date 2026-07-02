using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.Student;
using Application.Exceptions;
using Domain.Models;
using Domain.Models.View;
using Application.Interfaces;          
using Application.Services.Interfaces;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;      
        private readonly IMapper _mapper;
        private readonly IValidator<CreateStudentDto> _createValidator;
        private readonly IValidator<UpdateStudentDto> _updateValidator;
        private readonly IValidator<StudentExamReportFilter> _filterValidator;

        public StudentService(
            IUnitOfWork unitOfWork,                   
            IMapper mapper,
            IValidator<CreateStudentDto> createValidator,
            IValidator<UpdateStudentDto> updateValidator,
            IValidator<StudentExamReportFilter> filterValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
        }

        public IQueryable<StudentExamReport> GetStudentExamReportsQueryable()
        {
            return _unitOfWork.StudentExamReports.GetStudentExamReportsQueryable();
        }

        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _unitOfWork.Students.GetAllStudentsListAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan tələbə sistemdə tapılmadı!");
            return _mapper.Map<StudentDto>(student);
        }

        public async Task<IEnumerable<StudentDto>> GetTopStudentsAsync(int count)
        {
            var topStudents = await _unitOfWork.Students.GetTopStudentsAsync(count);
            return _mapper.Map<IEnumerable<StudentDto>>(topStudents);
        }

        public async Task<IEnumerable<StudentExamReport>> GetStudentReportAsync(StudentExamReportFilter filter)
        {
            await _filterValidator.ValidateAndThrowAsync(filter);

            if (!string.IsNullOrWhiteSpace(filter.StudentFullName))
            {
                var studentExists = await _unitOfWork.Students.IsStudentExistsByNameAsync(filter.StudentFullName);
                if (!studentExists)
                    throw new NotFoundException($"Axtarılan '{filter.StudentFullName}' adlı tələbə sistemdə mövcud deyil!");
            }

            return await _unitOfWork.StudentExamReports.GetFilteredReportsAsync(filter);
        }

        public async Task CreateStudentAsync(CreateStudentDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var student = _mapper.Map<Student>(dto);
            student.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {id} ID-li tələbə tapılmadı!");

            _mapper.Map(dto, student);
            student.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li tələbə tapılmadı!");

            student.Status = EntityStatus.Deleted;
            student.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Students.Update(student);
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