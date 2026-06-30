using FluentValidation;
using Application.Repositories;

namespace Application.DTOs.Department
{
    public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public UpdateDepartmentDtoValidator(
            IDepartmentRepository departmentRepository,
            IOrganizationRepository organizationRepository)
        {
            _departmentRepository = departmentRepository;
            _organizationRepository = organizationRepository;

            // 1. Sahə validasiyaları
            RuleFor(x => x.DepartmentName)
                .NotEmpty().WithMessage("Şöbə adı mütləq daxil edilməlidir!")
                .MaximumLength(150).WithMessage("Şöbə adı 150 simvoldan çox ola bilməz.");

            RuleFor(x => x.OrganizationId)
                .GreaterThan(0).WithMessage("Təşkilat ID-si 0-dan böyük olmalıdır.");

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage("Şöbə ID-si 0-dan böyük olmalıdır.");

            // 2. Biznes validasiyaları (asinxron)
            RuleFor(x => x.OrganizationId)
                .MustAsync(OrganizationExistsAsync)
                .WithMessage(x => $"Daxil edilən Təşkilat (ID: {x.OrganizationId}) tapılmadı!");

            
        }

        private async Task<bool> OrganizationExistsAsync(int organizationId, CancellationToken cancellationToken)
        {
            return await _organizationRepository.ExistsAsync(organizationId);
        }

       
    }
}