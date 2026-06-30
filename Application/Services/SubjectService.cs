using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Application.DTOs.Subject;
using Application.Exceptions;
using Domain.Models;
using Application.Repositories;
using Application.Services.Interfaces;

namespace Application.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSubjectDto> _createValidator;
        private readonly IValidator<UpdateSubjectDto> _updateValidator;

        public SubjectService(
            ISubjectRepository subjectRepository,
            IMapper mapper,
            IValidator<CreateSubjectDto> createValidator,
            IValidator<UpdateSubjectDto> updateValidator)
        {
            _subjectRepository = subjectRepository;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllSubjectsAsync()
        {
            var subjects = await _subjectRepository.GetAllSubjectsListAsync();
            return _mapper.Map<IEnumerable<SubjectDto>>(subjects);
        }

        public async Task<SubjectDto?> GetSubjectByIdAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null || subject.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan fənn sistemdə tapılmadı!");

            return _mapper.Map<SubjectDto>(subject);
        }

        public async Task<IEnumerable<SubjectDto>> GetHardestSubjectsAsync(int passingGrade = 51)
        {
            var hardestSubjects = await _subjectRepository.GetHardestSubjectsAsync(passingGrade);
            return _mapper.Map<IEnumerable<SubjectDto>>(hardestSubjects);
        }

        public async Task CreateSubjectAsync(CreateSubjectDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var subject = _mapper.Map<Subject>(dto);
            subject.Status = EntityStatus.Active;

            await _subjectRepository.AddAsync(subject);
            await _subjectRepository.SaveChangesAsync();
        }

        public async Task UpdateSubjectAsync(UpdateSubjectDto dto)
        {
            
            await _updateValidator.ValidateAndThrowAsync(dto);

            
            var subject = await _subjectRepository.GetByIdAsync(dto.SubjectId);
            if (subject == null || subject.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.SubjectId} ID-li fənn tapılmadı!");

            
            _mapper.Map(dto, subject);
            subject.UpdateDate = DateTime.UtcNow;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveChangesAsync();
        }

        public async Task DeleteSubjectAsync(int id)
        {
            var subject = await _subjectRepository.GetByIdAsync(id);
            if (subject == null || subject.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li fənn tapılmadı!");

            // Soft delete tətbiqi
            subject.Status = EntityStatus.Deleted;
            subject.UpdateDate = DateTime.UtcNow;

            _subjectRepository.Update(subject);
            await _subjectRepository.SaveChangesAsync();
        }
    }
}