using FluentValidation;

namespace Application.DTOs.UserRole
{
    public class CreateUserRoleDtoValidator : AbstractValidator<CreateUserRoleDto>
    {
        public CreateUserRoleDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("İstifadəçi mütləq düzgün seçilməlidir (UserId 0-dan böyük olmalıdır).");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Rol mütləq düzgün seçilməlidir (RoleId 0-dan böyük olmalıdır).");
        }
    }
}