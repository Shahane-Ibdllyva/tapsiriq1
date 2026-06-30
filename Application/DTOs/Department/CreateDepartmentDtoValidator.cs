using FluentValidation;
using Application.Repositories;
using Domain.Models;

namespace Application.DTOs.Department
{
    public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IOrganizationRepository _organizationRepository;

        public CreateDepartmentDtoValidator(
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

            // 2. Biznes validasiyaları (asinxron)
            RuleFor(x => x.OrganizationId)
                .MustAsync(OrganizationExistsAsync)
                .WithMessage(x => $"Daxil edilən Təşkilat (ID: {x.OrganizationId}) tapılmadı! Öncə təşkilatı yaratmalısınız.");

            RuleFor(x => x)
                .MustAsync(BeUniqueDepartmentNameAsync)
                .WithMessage(x => $"Bu təşkilatın daxilində '{x.DepartmentName}' adda şöbə artıq mövcuddur!");
        }

        private async Task<bool> OrganizationExistsAsync(int organizationId, CancellationToken cancellationToken)
        {
            return await _organizationRepository.ExistsAsync(organizationId);
        }

        private async Task<bool> BeUniqueDepartmentNameAsync(CreateDepartmentDto dto, CancellationToken cancellationToken)
        {
            return !await _departmentRepository.CheckDuplicateAsync(dto.OrganizationId, dto.DepartmentName);
        }
    }
}