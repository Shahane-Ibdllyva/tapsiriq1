using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.Organization;

namespace Application.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<IEnumerable<OrganizationDto>> GetAllOrganizationsAsync();
        Task<OrganizationDto?> GetOrganizationByIdAsync(int id);
        Task CreateOrganizationAsync(CreateOrganizationDto dto);
        Task UpdateOrganizationAsync(UpdateOrganizationDto dto); 
        Task DeleteOrganizationAsync(int id);
    }
}