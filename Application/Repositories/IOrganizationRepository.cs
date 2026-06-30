using Application.Repositories;
using Domain.Models;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<bool> ExistsAsync(int OrganizationId);
    Task<IEnumerable<Organization>> GetOrganizationsWithDepartmentsAsync();
    Task<Organization?> GetOrganizationByIdWithDepartmentsAsync(int id);
    Task<bool> CheckDuplicateNameAsync(string name);
    Task<bool> CheckDuplicateNameForUpdateAsync(string name, int excludeId); 
}