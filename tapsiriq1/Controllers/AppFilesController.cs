using Application.DTOs.AppFile;
using Application.Interfaces;        // <-- IUnitOfWork üçün
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace tapsiriq1.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppFilesController : ControllerBase
    {
        private readonly IAppFileService _fileService;
        private readonly IUnitOfWork _unitOfWork; // <-- Controller-ə əlavə olundu

        public AppFilesController(IAppFileService fileService, IUnitOfWork unitOfWork)
        {
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadProfilePictureDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 1;

            // Transaction başlat
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var createdFile = await _fileService.ProcessAndCreateAsync(dto.File, currentUserId);

                // Dəyişiklikləri bazaya yaz
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return StatusCode(201, new
                {
                    message = "Fayl uğurla bazaya yükləndi.",
                    fileId = createdFile.Id,
                    originalName = createdFile.OriginalName
                });
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFile(Guid id)
        {
            var appFile = await _fileService.GetFileByIdAsync(id);
            if (appFile == null)
            {
                return NotFound(new { message = "Fayl tapılmadı." });
            }

            return File(appFile.Content, appFile.ContentType, appFile.OriginalName);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int currentUserId = userIdClaim != null ? int.Parse(userIdClaim) : 1;

            // Transaction başlat (soft-delete də transaction tələb edir)
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _fileService.DeleteFileAsync(id, currentUserId);

                // Status dəyişikliyini bazaya yaz
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return Ok(new { message = "Fayl uğurla soft-delete olundu (Status = Deleted)." });
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}