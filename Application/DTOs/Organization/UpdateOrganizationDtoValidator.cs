using FluentValidation;
using Application.DTOs.Organization;

namespace Application.Validators.Organization
{
    public class UpdateOrganizationDtoValidator : AbstractValidator<UpdateOrganizationDto>
    {
        public UpdateOrganizationDtoValidator()
        {
           
            RuleFor(x => x.OrganizationId)
                .GreaterThan(0).WithMessage("Yenilənəcək təşkilatın ID-si düzgün (0-dan böyük) olmalıdır.");

          
            RuleFor(x => x.OrganizationName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Təşkilat adı mütləq daxil edilməlidir.")
                .MinimumLength(2).WithMessage("Təşkilat adı ən azı 2 simvoldan ibarət olmalıdır.")
                .MaximumLength(150).WithMessage("Təşkilat adı 150 simvoldan çox ola bilməz.");

            
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Təşkilatın ünvanı mütləq daxil edilməlidir.")
                .MaximumLength(250).WithMessage("Ünvan 250 simvoldan çox ola bilməz.");
        }
    }
}