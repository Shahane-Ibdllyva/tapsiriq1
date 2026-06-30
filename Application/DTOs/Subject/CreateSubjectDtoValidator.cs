using FluentValidation;

namespace Application.DTOs.Subject
{
    public class CreateSubjectDtoValidator : AbstractValidator<CreateSubjectDto>
    {
        public CreateSubjectDtoValidator()
        {
            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("Fənn kodu boş ola bilməz.")
                .MaximumLength(20).WithMessage("Fənn kodu 20 simvoldan çox ola bilməz.");

            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("Fənn adı boş ola bilməz.")
                .MaximumLength(100).WithMessage("Fənn adı 100 simvoldan çox ola bilməz.");

            RuleFor(x => x.TeacherName)
                .NotEmpty().WithMessage("Müəllim adı boş ola bilməz.")
                .MaximumLength(50).WithMessage("Müəllim adı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.TeacherSurname)
                .NotEmpty().WithMessage("Müəllim soyadı boş ola bilməz.")
                .MaximumLength(50).WithMessage("Müəllim soyadı 50 simvoldan çox ola bilməz.");

           
            RuleFor(x => x.ClassNumber)
                .GreaterThan(0).WithMessage("Sinif nömrəsi 0-dan böyük olmalıdır.")
                .When(x => x.ClassNumber.HasValue);
        }
    }
}