using FluentValidation;
using Application.DTOs.Organization;

namespace Application.Validators
{
    public class CreateOrganizationDtoValidator : AbstractValidator<CreateOrganizationDto>
    {
        public CreateOrganizationDtoValidator()
        {
            RuleFor(x => x.OrganizationName)
                .NotEmpty().WithMessage("Təşkilat adı boş ola bilməz!")
                .MaximumLength(150).WithMessage("Təşkilat adı 150 simvoldan çox ola bilməz.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Ünvan boş ola bilməz!")
                .MaximumLength(250).WithMessage("Ünvan 250 simvoldan çox ola bilməz.");
        }
    }
}