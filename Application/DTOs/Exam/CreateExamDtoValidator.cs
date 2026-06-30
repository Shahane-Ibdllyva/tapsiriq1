using FluentValidation;
using Application.Repositories;
using Domain.Models;

namespace Application.DTOs.Exam
{
    public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly IStudentRepository _studentRepository;

        public CreateExamDtoValidator(
            ISubjectRepository subjectRepository,
            IStudentRepository studentRepository)
        {
            _subjectRepository = subjectRepository;
            _studentRepository = studentRepository;

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Fənn ID-si 0-dan böyük olmalıdır.");

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Tələbə ID-si 0-dan böyük olmalıdır.");

            RuleFor(x => x.ExamDate)
                .NotEmpty().WithMessage("İmtahan tarixi boş ola bilməz.")
                .GreaterThan(DateTime.Now.AddDays(-1))
                .WithMessage("İmtahan tarixi keçmiş ola bilməz.");

            RuleFor(x => x.Grade)
                .InclusiveBetween(0, 100).When(x => x.Grade.HasValue)
                .WithMessage("Qiymət 0-100 arasında olmalıdır.");

            RuleFor(x => x.SubjectId)
                .MustAsync(SubjectExistsAsync)
                .WithMessage(x => $"Daxil edilən Fənn (ID: {x.SubjectId}) tapılmadı!");

            RuleFor(x => x.StudentId)
                .MustAsync(StudentExistsAsync)
                .WithMessage(x => $"Daxil edilən Tələbə (ID: {x.StudentId}) tapılmadı!");
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