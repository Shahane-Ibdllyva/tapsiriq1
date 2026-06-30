using FluentValidation;
using Application.DTOs.Subject;

namespace Application.DTOs.Subject
{
    public class UpdateSubjectDtoValidator : AbstractValidator<UpdateSubjectDto>
    {
        public UpdateSubjectDtoValidator()
        {
            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Yenilənəcək fənnin ID-si düzgün daxil edilməlidir.");

            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("Fənn kodu mütləq daxil edilməlidir.")
                .MaximumLength(10).WithMessage("Fənn kodu 10 simvoldan çox ola bilməz.");

            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("Fənn adı mütləq daxil edilməlidir.")
                .MaximumLength(100).WithMessage("Fənn adı 100 simvoldan çox ola bilməz.");

            RuleFor(x => x.ClassNumber)
                .InclusiveBetween(1, 12).When(x => x.ClassNumber.HasValue)
                .WithMessage("Sinif nömrəsi 1 ilə 12 arasında olmalıdır.");

            RuleFor(x => x.TeacherName)
                .NotEmpty().WithMessage("Müəllim adı mütləq qeyd olunmalıdır.");

            RuleFor(x => x.TeacherSurname)
                .NotEmpty().WithMessage("Müəllim soyadı mütləq qeyd olunmalıdır.");
        }
    }
}