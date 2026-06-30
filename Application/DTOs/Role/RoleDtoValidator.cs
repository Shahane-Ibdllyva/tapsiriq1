using FluentValidation;
using Application.DTOs.Role;

namespace Application.Validators.Role
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol adı mütləq daxil edilməlidir.")
                .MinimumLength(2).WithMessage("Rol adı ən azı 2 simvoldan ibarət olmalıdır.")
                .MaximumLength(50).WithMessage("Rol adı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Təsvir 250 simvoldan çox ola bilməz.");
        }
    }

    public class UpdateRoleDtoValidator : AbstractValidator<UpdateRoleDto>
    {
        public UpdateRoleDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Yenilənəcək rolun ID-si düzgün daxil edilməlidir.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol adı mütləq daxil edilməlidir.")
                .MinimumLength(2).WithMessage("Rol adı ən azı 2 simvoldan ibarət olmalıdır.")
                .MaximumLength(50).WithMessage("Rol adı 50 simvoldan çox ola bilməz.");

            RuleFor(x => x.Description)
                .MaximumLength(250).WithMessage("Təsvir 250 simvoldan çox ola bilməz.");

            RuleFor(x => x.Status)
                .NotNull().WithMessage("Status mütləq göndərilməlidir.");
        }
    }
}