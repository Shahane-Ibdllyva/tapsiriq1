using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.Student;
using Application.Exceptions;
using Domain.Models;
using Domain.Models.View;
using Application.Repositories;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IViewRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateStudentDto> _createValidator;
        private readonly IValidator<UpdateStudentDto> _updateValidator;
        private readonly IValidator<StudentExamReportFilter> _filterValidator;

        public StudentService(
            IStudentRepository studentRepository,
            IViewRepository reportRepository,
            IMapper mapper,
            IValidator<CreateStudentDto> createValidator,
            IValidator<UpdateStudentDto> updateValidator,
            IValidator<StudentExamReportFilter> filterValidator)
        {
            _studentRepository = studentRepository;
            _reportRepository = reportRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _filterValidator = filterValidator;
        }


        public IQueryable<StudentExamReport> GetStudentExamReportsQueryable()
        {
            return _reportRepository.GetStudentExamReportsQueryable();
        }


        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            var students = await _studentRepository.GetAllStudentsListAsync();
            return _mapper.Map<IEnumerable<StudentDto>>(students);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan tələbə sistemdə tapılmadı!");

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<IEnumerable<StudentDto>> GetTopStudentsAsync(int count)
        {
            var topStudents = await _studentRepository.GetTopStudentsAsync(count);
            return _mapper.Map<IEnumerable<StudentDto>>(topStudents);
        }

        public async Task<IEnumerable<StudentExamReport>> GetStudentReportAsync(StudentExamReportFilter filter)
        {
           
            await _filterValidator.ValidateAndThrowAsync(filter);

            // 2. Biznes yoxlanışı: Əgər Tələbə adı ilə axtarış verilibsə və bazada yoxdursa
            if (!string.IsNullOrWhiteSpace(filter.StudentFullName))
            {
                var studentExists = await _studentRepository.IsStudentExistsByNameAsync(filter.StudentFullName);

                if (!studentExists)
                    throw new NotFoundException($"Axtarılan '{filter.StudentFullName}' adlı tələbə sistemdə mövcud deyil!");
            }

            return await _reportRepository.GetFilteredReportsAsync(filter);
        }

        public async Task CreateStudentAsync(CreateStudentDto dto)
        {
          
            await _createValidator.ValidateAndThrowAsync(dto);

            // 2. AutoMapper tətbiqi (Fərqli property adları profile daxilində idarə olunur)
            var student = _mapper.Map<Student>(dto);
            student.Status = EntityStatus.Active;

            await _studentRepository.AddAsync(student);
            await _studentRepository.SaveChangesAsync();
        }

        public async Task UpdateStudentAsync(int id, UpdateStudentDto dto)
        {
            // 1. Formal Validasiya
            await _updateValidator.ValidateAndThrowAsync(dto);

            // 2. Mövcudluq yoxlanışı
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {id} ID-li tələbə tapılmadı!");

            // 3. AutoMapper ilə birbaşa mövcud entity üzərinə map edirik
            _mapper.Map(dto, student);
            student.UpdateDate = DateTime.UtcNow;

            _studentRepository.Update(student);
            await _studentRepository.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null || student.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li tələbə tapılmadı!");

            // Soft delete tətbiqi
            student.Status = EntityStatus.Deleted;
            student.UpdateDate = DateTime.UtcNow;

            _studentRepository.Update(student);
            await _studentRepository.SaveChangesAsync();
        }
    }
}