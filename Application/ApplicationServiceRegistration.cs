using Application.DTOs.Auth;
using Application.DTOs.Department;
using Application.DTOs.Exam;
using Application.DTOs.Organization;
using Application.DTOs.Subject;
using Application.DTOs.User;
using Application.DTOs.UserRole;
using Application.Mappings;
using Application.Services;
using Application.Services.Interfaces;
using Application.Validators;
using FluentValidation;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();
        //services.AddScoped<IValidator<UpdateDepartmentDto>, UpdateDepartmentDtoValidator>();
        //services.AddScoped<IValidator<CreateDepartmentDto>, CreateDepartmentDtoValidator>();
        //services.AddScoped<IValidator<CreateExamDto>, CreateExamDtoValidator>();
        //services.AddScoped<IValidator<UpdateExamDto>, UpdateExamDtoValidator>();
        //services.AddScoped<IValidator<CreateOrganizationDto>, CreateOrganizationDtoValidator>();
        //services.AddScoped<IValidator<UpdateSubjectDto>, UpdateSubjectDtoValidator>();
        //services.AddScoped<IValidator<CreateSubjectDto>, CreateSubjectDtoValidator>();
        //services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();
        //services.AddScoped<IValidator<UpdateUserDto>, UpdateUserDtoValidator>();


        //services.AddScoped<IValidator<CreateUserRoleDto>, CreateUserRoleDtoValidator>();
        //services.AddScoped<IValidator<UpdateUserRoleDto>, UpdateUserRoleDtoValidator>();


        // Biznes Servisləri
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<IExamService, ExamService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IAppFileService, AppFileService>();

        return services;
    }
}