using Application.DTOs.Department;
using Application.DTOs.Exam;
using Application.DTOs.Organization;
using Application.DTOs.Role; // Əgər eyni fayldadırsa qalsın
using Application.DTOs.Student;
using Application.DTOs.Subject;
using Application.DTOs.User;
using Application.DTOs.UserRole;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Role Mappings (Əvvəlki) ---
            CreateMap<Role, RoleListDto>();
            CreateMap<CreateRoleDto, Role>();
            CreateMap<UpdateRoleDto, Role>();

          
            CreateMap<CreateDepartmentDto, Department>();

            
            CreateMap<UpdateDepartmentDto, Department>();

            // Əgər ehtiyac olarsa deyə Department -> DepartmentListDto (Tərsini də aktiv edə bilərsən)
            CreateMap<Department, DepartmentListDto>();
            CreateMap<Exam, ExamDto>();
            CreateMap<CreateExamDto, Exam>();
          
            CreateMap<UpdateExamDto, Exam>();
            CreateMap<CreateOrganizationDto, Organization>();
            CreateMap<UpdateOrganizationDto, Organization>();
            CreateMap<Organization, OrganizationDto>();
            CreateMap<CreateRoleDto, Role>();
            CreateMap<UpdateRoleDto, Role>();
            CreateMap<Role, RoleListDto>();
            // --- Student Mappings ---

            // 1. CreateStudentDto -> Student
            CreateMap<CreateStudentDto, Student>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.StudentSurname, opt => opt.MapFrom(src => src.LastName));

            // 2. UpdateStudentDto -> Student
            CreateMap<UpdateStudentDto, Student>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.StudentSurname, opt => opt.MapFrom(src => src.LastName));

            // 3. Student -> StudentDto
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.StudentName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.StudentSurname));
            // --- Subject Mappings ---
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<UpdateSubjectDto, Subject>();
            CreateMap<Subject, SubjectDto>();
            CreateMap<UpdateSubjectDto, SubjectDto>();
            // --- User Mappings ---
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); 
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UserListDto>();

            CreateMap<CreateUserRoleDto, UserRole>();
            CreateMap<UpdateUserRoleDto, UserRole>();
           
        }
    }
}