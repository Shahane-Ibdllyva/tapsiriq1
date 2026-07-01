using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using tapsiriq1.Domain.Entities;
using Application.DTOs.AppFile;

namespace Application.Services
{
    public interface IAppFileService
    {
        // Gələn IFormFile-ı validasiya edib, byte[]-a çevirərək bazaya yazır
        Task<AppFile> ProcessAndCreateAsync(IFormFile file, int currentUserId);

       
        Task<AppFile?> GetFileByIdAsync(Guid fileId);

       
       
        Task DeleteFileAsync(Guid fileId, int currentUserId);
    }
}