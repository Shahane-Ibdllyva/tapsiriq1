using Microsoft.AspNetCore.Http;
using System;

namespace Application.DTOs.AppFile
{
    // GET əməliyyatlarında faylın metadatasını frontend-ə göstərmək üçün
    public class AppFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
    }

    // Əgər frontend bizə birbaşa bayt massivi göndərmək istəsə (Mövcud strukturunuz)
    public class AppFileUploadDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }

    // Memarlığa və image_71109a.png şəklinə tam uyğun olaraq: Form-data üzərindən şəkil yükləmək üçün DTO
    public class UploadProfilePictureDto
    {
        public IFormFile File { get; set; } = null!;
    }

    public class UpdateProfilePictureDto
    {
        public IFormFile? File { get; set; }
    }
}