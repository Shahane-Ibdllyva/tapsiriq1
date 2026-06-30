using FluentValidation;

namespace Application.DTOs.User
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad mütləq daxil edilməlidir!")
                .MaximumLength(50).WithMessage("Ad 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Soyad mütləq daxil edilməlidir!")
                .MaximumLength(50).WithMessage("Soyad 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-poçt ünvanı mütləq daxil edilməlidir!")
                .EmailAddress().WithMessage("Düzgün bir e-poçt ünvanı daxil edin.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Şöbə ID-si düzgün seçilməlidir!");
        }
    }
}