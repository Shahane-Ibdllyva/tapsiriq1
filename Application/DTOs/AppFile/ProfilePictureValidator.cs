using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Application.DTOs.AppFile
{
    
    public class CoreFileValidator : AbstractValidator<IFormFile>
    {
        public CoreFileValidator()
        {
            
            RuleFor(file => file)
                .NotNull().WithMessage("Yüklənilən şəkil boş ola bilməz.")
                .NotEmpty().WithMessage("Yüklənilən şəkil boş ola bilməz.");

           
            RuleFor(file => file.Length)
                .LessThanOrEqualTo(2 * 1024 * 1024)
                .WithMessage("Profil şəklininin ölçüsü maksimum 2 MB ola bilər.");

           
            //RuleFor(file => file.FileName)
            //    .Must(HaveAllowedExtension)
            //    .WithMessage("Yalnız .jpg, .jpeg, .png və .webp formatında şəkillər qəbul edilir.");
        }

        private bool HaveAllowedExtension(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;

            var extension = Path.GetExtension(fileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".doc" };

            return allowedExtensions.Contains(extension);
        }
    }

   
    public class UploadProfilePictureDtoValidator : AbstractValidator<UploadProfilePictureDto>
    {
        public UploadProfilePictureDtoValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("Profil şəkli mütləq yüklənməlidir.")
                .SetValidator(new CoreFileValidator());
        }
    }

    public class UpdateProfilePictureDtoValidator : AbstractValidator<UpdateProfilePictureDto>
    {
        public UpdateProfilePictureDtoValidator()
        {
            When(x => x.File != null, () => {
                RuleFor(x => x.File!).SetValidator(new CoreFileValidator());
            });
        }
    }
}