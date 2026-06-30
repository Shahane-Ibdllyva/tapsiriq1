using FluentValidation;
using Application.Repositories;
using Domain.Models;

namespace Application.DTOs.Exam
{
    public class UpdateExamDtoValidator : AbstractValidator<UpdateExamDto>
    {
        private readonly IExamRepository _examRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IStudentRepository _studentRepository;

        public UpdateExamDtoValidator(
            IExamRepository examRepository,
            ISubjectRepository subjectRepository,
            IStudentRepository studentRepository)
        {
            _examRepository = examRepository;
            _subjectRepository = subjectRepository;
            _studentRepository = studentRepository;

            RuleFor(x => x.ExamId)
                .GreaterThan(0).WithMessage("İmtahan ID-si 0-dan böyük olmalıdır.");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("İmtahan tarixi boş ola bilməz.");
                //.GreaterThan(DateTime.Now.AddDays(-1))
                //.WithMessage("İmtahan tarixi keçmiş ola bilməz.");

            RuleFor(x => x.Grade)
                .InclusiveBetween(0, 100).When(x => x.Grade.HasValue)
                .WithMessage("Qiymət 0-100 arasında olmalıdır.");

            // Exam-in mövcudluğu
            RuleFor(x => x)
                .MustAsync(ExamExistsAsync)
                .WithMessage(x => $"Yenilənəcək {x.ExamId} ID-li imtahan tapılmadı!");

            // SubjectId yoxlanışı (əgər göndərilibsə)
            When(x => x.SubjectId.HasValue, () =>
            {
                RuleFor(x => x.SubjectId.Value)
                    .MustAsync(SubjectExistsAsync)
                    .WithMessage(x => $"Daxil edilən Fənn (ID: {x}) tapılmadı!");
            });

            // StudentId yoxlanışı (əgər göndərilibsə)
            When(x => x.StudentId.HasValue, () =>
            {
                RuleFor(x => x.StudentId.Value)
                    .MustAsync(StudentExistsAsync)
                    .WithMessage(x => $"Daxil edilən Tələbə (ID: {x}) tapılmadı!");
            });
        }

        private async Task<bool> ExamExistsAsync(UpdateExamDto dto, CancellationToken cancellationToken)
        {
            var exam = await _examRepository.GetByIdAsync(dto.ExamId);
            return exam != null && exam.Status != EntityStatus.Deleted;
        }

        private async Task<bool> SubjectExistsAsync(int subjectId, CancellationToken cancellationToken)
        {
            return await _subjectRepository.ExistsAsync(subjectId);
        }

        private async Task<bool> StudentExistsAsync(int studentId, CancellationToken cancellationToken)
        {
            return await _studentRepository.ExistsAsync(studentId);
        }
    }
}