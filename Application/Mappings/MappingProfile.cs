using Application.DTOs;
using Application.DTOs.AppFile;
using Application.DTOs.Department;
using Application.DTOs.Exam;
using Application.DTOs.Organization;
using Application.DTOs.Role; 
using Application.DTOs.Student;
using Application.DTOs.Subject;
using Application.DTOs.User;
using Application.DTOs.UserRole;
using AutoMapper;
using Domain.Models;
using tapsiriq1.Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           
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
            CreateMap<AppFile, AppFileDto>().ReverseMap();
            CreateMap<AppFileUploadDto, AppFile>();
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
           
            // --- AppFile Mappings ---
            // Əgər ayrıca AppFileDto istifadə etməyəcəksənsə bunları saxlaya da bilərsən, silə də bilərsən.
            CreateMap<AppFile, AppFileDto>().ReverseMap();
            CreateMap<AppFileUploadDto, AppFile>();

            // --- User Mappings (YENİLƏNDİ) ---
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.AppFile, opt => opt.Ignore()); // Şəkil əməliyyatı servisdə olacaq

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.AppFile, opt => opt.Ignore()); // Şəkil əməliyyatı servisdə olacaq

            // User-dən UserListDto-ya keçəndə şəklin ID-sini təhlükəsiz şəkildə mənimsədirik
            CreateMap<User, UserListDto>()
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.AppFile != null ? src.AppFile.Id : (System.Guid?)null));

        }
    }
}