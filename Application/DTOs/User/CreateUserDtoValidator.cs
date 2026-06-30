using FluentValidation;

namespace Application.DTOs.User
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
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

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("İstifadəçi adı mütləq daxil edilməlidir!")
                .MaximumLength(100).WithMessage("İstifadəçi adı 100 simvoldan çox ola bilməz.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifrə mütləq daxil edilməlidir!")
                .MinimumLength(6).WithMessage("Şifrə ən azı 6 simvoldan ibarət olmalıdır.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("İstifadəçinin aid olacağı şöbə mütləq seçilməlidir!");
        }
    }
}