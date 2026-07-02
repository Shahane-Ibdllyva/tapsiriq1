using Application.DTOs.Exam;
using Application.Exceptions;
using Application.Interfaces;          
using Application.Services.Interfaces;
using Domain.Models;
using FluentValidation;
using AutoMapper;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;      
        private readonly IValidator<CreateExamDto> _createValidator;
        private readonly IValidator<UpdateExamDto> _updateValidator;
        private readonly IMapper _mapper;

        public ExamService(
            IUnitOfWork unitOfWork,                   
            IValidator<CreateExamDto> createValidator,
            IValidator<UpdateExamDto> updateValidator,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ExamDto>> GetAllExamsAsync()
        {
            var exams = await _unitOfWork.Exams.GetExamsWithDetailsAsync();
            return _mapper.Map<IEnumerable<ExamDto>>(exams);
        }

        public async Task<ExamDto?> GetExamByIdAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetExamByIdWithDetailsAsync(id);

            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"ID-si {id} olan imtahan sistemdə tapılmadı!");

            return _mapper.Map<ExamDto>(exam);
        }

        public async Task CreateExamAsync(CreateExamDto dto)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var entity = _mapper.Map<Exam>(dto);
            entity.Grade = dto.Grade ?? 0;
            entity.Status = EntityStatus.Active;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Exams.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdateExamAsync(UpdateExamDto dto)
        {
            await _updateValidator.ValidateAndThrowAsync(dto);

            var exam = await _unitOfWork.Exams.GetByIdAsync(dto.ExamId);
            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Yenilənəcək {dto.ExamId} ID-li imtahan tapılmadı!");

            _mapper.Map(dto, exam);
            exam.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Exams.Update(exam);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeleteExamAsync(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null || exam.Status == EntityStatus.Deleted)
                throw new NotFoundException($"Silinəcək {id} ID-li imtahan tapılmadı!");

            exam.Status = EntityStatus.Deleted;
            exam.UpdateDate = DateTime.UtcNow;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Exams.Update(exam);
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