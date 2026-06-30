using Application.DTOs.Exam;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Models;
using FluentValidation;
using AutoMapper;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly IValidator<CreateExamDto> _createValidator;
        private readonly IValidator<UpdateExamDto> _updateValidator;
        private readonly IMapper _mapper;

        public ExamService(
            IExamRepository examRepository,
            IValidator<CreateExamDto> createValidator,
            IValidator<UpdateExamDto> updateValidator,
            IMapper mapper)
        {
            _examRepository = examRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _examRepository.GetExamsWithDetailsAsync();

            // Siyahını tək sətirdə ExamDto siyahısına çeviririk
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetExamByIdWithDetailsAsync(id);

            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan imtahan sistemdə tapılmadı!");

            return _mapper.Map<ExamDto>(exam);
        }

        public async Task CreateExamAsync(CreateExamDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            // AutoMapper tətbiqi: CreateExamDto -> Exam entity
            var entity = _mapper.Map<Exam>(dto);

            // DTO-da olmayan default dəyərləri əllə set edirik
            entity.Grade = dto.Grade ?? 0;
            entity.Status = EntityStatus.Active;

            await _examRepository.AddAsync(entity);
            await _examRepository.SaveChangesAsync();
        }

        public async Task UpdateExamAsync(UpdateExamDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.ExamId} ID-li imtahan tapılmadı!");

            // Mövcut Entity-nin üzərinə DTO-dan gələn dataları map edirik.
            // Bu sətir null olmayan və ötürülən bütün dəyərləri avtomatik bərabərləşdirir.
            _mapper.Map(dto, exam);

            exam.UpdateDate = DateTime.UtcNow;

            _examRepository.Update(exam);
            await _examRepository.SaveChangesAsync();
        }

        public async Task DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li imtahan tapılmadı!");

            exam.Status = EntityStatus.Deleted;
            exam.UpdateDate = DateTime.UtcNow;

            _examRepository.Update(exam);
            await _examRepository.SaveChangesAsync();
        }
    }
}