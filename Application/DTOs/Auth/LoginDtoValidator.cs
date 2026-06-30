using FluentValidation;
using Application.Repositories;
using Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace Application.DTOs.Auth   
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        private readonly IUserRepository _userRepository;

        public LoginDtoValidator(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email ünvanı boş ola bilməz.")
                .EmailAddress().WithMessage("Düzgün email formatı daxil edin.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Şifrə boş ola bilməz.")
                .MinimumLength(6).WithMessage("Şifrə minimum 6 simvol olmalıdır.");

            RuleFor(x => x)
                .MustAsync(BeValidUser)
                .WithMessage("E-poçt ünvanı və ya şifrə yanlışdır!");
        }

        private async Task<bool> BeValidUser(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || user.Status == EntityStatus.Deleted)
                return false;

            string incomingPasswordHash = ComputeMD5Hash(dto.Password);
            return user.PasswordHash == incomingPasswordHash;
        }

        private string ComputeMD5Hash(string password)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}