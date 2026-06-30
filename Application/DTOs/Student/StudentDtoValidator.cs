using FluentValidation;
using Application.DTOs.Student;

namespace Application.Validators.Student
{
    public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
    {
        public CreateStudentDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Tələbənin adı mütləq daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Tələbənin adı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tələbənin soyadı mütləq daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Tələbənin soyadı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.ClassNumber)
                .InclusiveBetween(1, 12).When(x => x.ClassNumber.HasValue)
                .WithMessage("Sinif nömrəsi 1 ilə 12 arasında olmalıdır.");
        }
    }

    public class UpdateStudentDtoValidator : AbstractValidator<UpdateStudentDto>
    {
        public UpdateStudentDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Tələbənin adı mütləq daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Tələbənin adı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tələbənin soyadı mütləq daxil edilməlidir.")
                .MaximumLength(50).WithMessage("Tələbənin soyadı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.ClassNumber)
                .InclusiveBetween(1, 12).When(x => x.ClassNumber.HasValue)
                .WithMessage("Sinif nömrəsi 1 ilə 12 arasında olmalıdır.");
        }
    }
}