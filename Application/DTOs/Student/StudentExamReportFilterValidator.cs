using FluentValidation;
using Application.DTOs.Student;

namespace Application.Validators.Student
{
    public class StudentExamReportFilterValidator : AbstractValidator<StudentExamReportFilter>
    {
        public StudentExamReportFilterValidator()
        {
            // Qiymət format yoxlanışları
            RuleFor(x => x.GradeMin)
                .InclusiveBetween(0, 100).When(x => x.GradeMin.HasValue)
                .WithMessage("Minimum qiymət 0-dan kiçik və ya 100-dən böyük ola bilməz.");

            RuleFor(x => x.GradeMax)
                .InclusiveBetween(0, 100).When(x => x.GradeMax.HasValue)
                .WithMessage("Maksimum qiymət 0-dan kiçik və ya 100-dən böyük ola bilməz.")
                .GreaterThanOrEqualTo(x => x.GradeMin.Value).When(x => x.GradeMin.HasValue && x.GradeMax.HasValue)
                .WithMessage("Maksimum qiymət minimum qiymətdən kiçik ola bilməz.");

            // Tarix yoxlanışı
            RuleFor(x => x.ExamDateMax)
                .GreaterThanOrEqualTo(x => x.ExamDateMin.Value).When(x => x.ExamDateMin.HasValue && x.ExamDateMax.HasValue)
                .WithMessage("Maksimum imtahan tarixi minimum tarixdən əvvəl ola bilməz.");
        }
    }
}