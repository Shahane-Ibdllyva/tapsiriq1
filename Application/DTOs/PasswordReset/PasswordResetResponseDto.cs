using System;

namespace Application.DTOs.PasswordReset
{
    public class PasswordResetResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}