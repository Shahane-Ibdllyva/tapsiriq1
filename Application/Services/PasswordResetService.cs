using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PasswordReset;
using Application.Exceptions;
using Application.Interfaces;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ILogger<PasswordResetService> _logger;

        public PasswordResetService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ILogger<PasswordResetService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _logger = logger;
        }

        /// 1. Addım: İstifadəçiyə UTP kodu göndər
        public async Task<PasswordResetResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto dto)
        {
            // 1. İstifadəçini email ilə tap
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException("Bu e-poçt ünvanı ilə qeydiyyatdan keçmiş istifadəçi tapılmadı!");

            // 2. İstifadəçinin əvvəlki aktiv token-lərini ləğv et (təhlükəsizlik üçün)
            var existingToken = await _unitOfWork.PasswordResetTokens.GetLastValidTokenByUserIdAsync(user.UserId);
            if (existingToken != null)
            {
                existingToken.IsRevoked = true;
                existingToken.UpdateDate = DateTime.UtcNow;
                _unitOfWork.PasswordResetTokens.Update(existingToken);
                await _unitOfWork.SaveChangesAsync();
            }

            // 3. 6 rəqəmli UTP kodu yarat
            var token = GenerateOtp();

            // 4. Token obyekti yarat (5 dəqiqə bitmə müddəti)
            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5), // 5 dəqiqə
                IsUsed = false,
                IsRevoked = false,
                InsertDate = DateTime.UtcNow,
                InsertByUserId = user.UserId,
                Status = EntityStatus.Active
            };

            // 5. Token-i bazaya yaz
            await _unitOfWork.PasswordResetTokens.AddAsync(resetToken);
            await _unitOfWork.SaveChangesAsync();

            // 6. Email göndər (Transaction-dan KƏNARDA - API bloklanmasın)
            try
            {
                var fullName = $"{user.FirstName} {user.LastName}";
                var body = _emailTemplateService.GetPasswordResetEmail(fullName, token);

                await _emailService.SendEmailAsync(
                    user.Email,
                    "🔐 Şifrə Sıfırlama - Təsdiq Kodu (UTP)",
                    body,
                    isHtml: true
                );

                _logger.LogInformation($"✅ Şifrə sıfırlama kodu ({token}) {user.Email} ünvanına göndərildi.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"❌ Şifrə sıfırlama emaili göndərilmədi: {user.Email}");
                // Email uğursuz olsa belə, token bazada qalır. İstifadəçi yenidən cəhd edə bilər.
                // Lakin istəsəniz burada xətanı ata bilərsiniz.
            }

            return new PasswordResetResponseDto
            {
                IsSuccess = true,
                Message = "6 rəqəmli təsdiq kodu email ünvanınıza göndərildi. Kod 5 dəqiqə ərzində etibarlıdır.",
                ExpiryDate = resetToken.ExpiryDate
            };
        }

       
        /// 2. Addım: UTP kodunu təsdiqlə və şifrəni yenilə
        public async Task<PasswordResetResponseDto> ConfirmResetPasswordAsync(PasswordResetConfirmDto dto)
        {
            // 1. Token-i bazada tap (istifadə olunmamış və ləğv edilməmiş)
            var resetToken = await _unitOfWork.PasswordResetTokens.GetByTokenAsync(dto.Token);
            if (resetToken == null)
                throw new BadRequestException("Yanlış və ya istifadə olunmuş təsdiq kodu!");

            // 2. Vaxt yoxlaması (5 dəqiqə)
            if (resetToken.ExpiryDate < DateTime.UtcNow)
                throw new BadRequestException("Təsdiq kodunun etibarlılıq müddəti bitmişdir. Yenidən kod istəyin!");

            // 3. Kod artıq istifadə olunub?
            if (resetToken.IsUsed)
                throw new BadRequestException("Bu kod artıq istifadə olunmuşdur!");

            // 4. Kod ləğv edilib?
            if (resetToken.IsRevoked)
                throw new BadRequestException("Bu kod ləğv edilmişdir. Yenidən kod istəyin!");

            // 5. İstifadəçini tap
            var user = await _unitOfWork.Users.GetByIdAsync(resetToken.UserId);
            if (user == null || user.Status == EntityStatus.Deleted)
                throw new NotFoundException("İstifadəçi tapılmadı!");

            // 6. Yeni şifrəni hash-lə
            user.PasswordHash = ComputeMD5Hash(dto.NewPassword); 
            user.UpdateDate = DateTime.UtcNow;

            // 7. Token-i istifadə olunmuş kimi işarələ
            resetToken.IsUsed = true;
            resetToken.UpdateDate = DateTime.UtcNow;

            // 8. Transaction ilə dəyişiklikləri yadda saxla
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Users.Update(user);
                _unitOfWork.PasswordResetTokens.Update(resetToken);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation($"✅ {user.Email} üçün şifrə uğurla sıfırlandı. Token: {dto.Token}");
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

            return new PasswordResetResponseDto
            {
                IsSuccess = true,
                Message = "Şifrə uğurla sıfırlandı. Yeni şifrə ilə daxil ola bilərsiniz."
            };
        }


        /// 6 rəqəmli təsadüfi UTP kodu yaradır (məs: 482910)
        private string GenerateOtp()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[4]; // 4 bayt = 32 bit
            rng.GetBytes(bytes);
            int randomNumber = BitConverter.ToInt32(bytes, 0) & int.MaxValue; // Müsbət ədəd üçün
            int otp = 100000 + (randomNumber % 900000); // 100000 - 999999 aralığı
            return otp.ToString();
        }

        /// MD5 hash - YALNIZ TEST ÜÇÜN! İstehsalatda BCrypt və ya Argon2 istifadə edin.
        private string ComputeMD5Hash(string password)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}