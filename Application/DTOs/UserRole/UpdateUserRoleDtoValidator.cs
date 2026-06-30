using FluentValidation;

namespace Application.DTOs.UserRole
{
    public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("İstifadəçi mütləq düzgün seçilməlidir.");

            RuleFor(x => x.RoleId)
                .GreaterThan(0).WithMessage("Rol mütləq düzgün seçilməlidir.");

            RuleFor(x => x.Status)
                .InclusiveBetween(0, 3).WithMessage("Status dəyəri düzgün deyil."); 
        }
    }
}