using System;
using Domain.Models; 
namespace Domain.Entities
{
    public class PasswordResetToken : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        // 6 rəqəmli UTP kodu (string kimi saxlayırıq, çünki bəzən 0-la başlaya bilər)
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
    }
}