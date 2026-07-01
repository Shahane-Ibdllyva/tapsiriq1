using Application.DTOs.AppFile;
using Application.Exceptions;
using Application.Interfaces;           
using Application.Services;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using tapsiriq1.Domain.Entities;

namespace Infrastructure.Services
{
    public class AppFileService : IAppFileService
    {
        private readonly IUnitOfWork _unitOfWork;           
        private readonly IValidator<IFormFile> _fileValidator;

        public AppFileService(IUnitOfWork unitOfWork, IValidator<IFormFile> fileValidator)
        {
            _unitOfWork = unitOfWork;
            _fileValidator = fileValidator;
        }

        public async Task<AppFile> ProcessAndCreateAsync(IFormFile file, int currentUserId)
        {
           
            var validationResult = await _fileValidator.ValidateAsync(file);
            if (!validationResult.IsValid)
            {
                var firstError = validationResult.Errors.First().ErrorMessage;
                throw new BadRequestException(firstError);
            }

           
            var isDuplicate = _unitOfWork.AppFiles.GetAll()
                .Any(f => f.OriginalName == file.FileName && f.Size == file.Length);

            if (isDuplicate)
            {
                throw new DuplicateNotificationException($"Əsas bildiriş: '{file.FileName}' faylı duplikatdır! Sistemdə artıq mövcuddur.");
            }

            // 3. Faylı byte[]-a çevir
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            byte[] fileContent = memoryStream.ToArray();

            var appFile = new AppFile
            {
                Id = Guid.NewGuid(),
                Content = fileContent,
                OriginalName = file.FileName,
                Extension = Path.GetExtension(file.FileName).ToLower(),
                ContentType = file.ContentType,
                Size = file.Length,
                InsertDate = DateTime.UtcNow,
                InsertByUserId = currentUserId
            };

           
            await _unitOfWork.AppFiles.AddAsync(appFile);

            return appFile; // Xarici tərəf SaveChanges-i çağıracaq
        }

        public async Task<AppFile?> GetFileByIdAsync(Guid fileId)
        {
            return await _unitOfWork.AppFiles.GetByIdAsync(fileId);
        }

        public async Task DeleteFileAsync(Guid fileId, int currentUserId)
        {
            var file = await _unitOfWork.AppFiles.GetByIdAsync(fileId);
            if (file == null)
            {
                throw new NotFoundException("Fayl tapılmadı.");
            }

            
            file.Status = EntityStatus.Deleted;
            file.UpdateDate = DateTime.UtcNow;
            file.UpdateByUserId = currentUserId;

            // Yenilə, amma SaveChanges-i xarici tərəf çağıracaq
            _unitOfWork.AppFiles.Update(file);
        }
    }
}