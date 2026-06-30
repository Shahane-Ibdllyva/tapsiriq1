using FluentValidation;
using Application.DTOs.Auth;
using Application.Exceptions;
using Application.Repositories;
using Application.Services.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IConfiguration _configuration;
        private readonly IValidator<LoginDto> _validator;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            IUserRoleRepository userRoleRepository,
            IValidator<LoginDto> validator) 
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userRoleRepository = userRoleRepository;
            _validator = validator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            // 1. BÜTÜN VALİDASİYA QAYDALARINI BİR XƏTTƏ YOXLA
            // Uğursuz olarsa ValidationException atar, middleware handle edəcək
            await _validator.ValidateAndThrowAsync(dto);

            // 2. İstifadəçini tap – burada artıq heç bir if yoxlanmır,
            // çünki validator zəmanət verir ki, user var, aktivdir və şifrə doğrudur.
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            // 3. Rolları bazadan çək
            var roles = await _userRoleRepository.GetRolesByUserIdAsync(user.UserId);

            // 4. JWT token yarat
            string token = GenerateJwtToken(user, roles);

            return new LoginResponseDto
            {
                Token = token,
                Username = user.Username
            };
        }

        // GenerateJwtToken metodu eyni qalır (heç bir dəyişiklik yoxdur)
        private string GenerateJwtToken(User user, List<Role> roles)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            var claims = new List<Claim>
            {
                new Claim("userId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"]!)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // ComputeMD5Hash metodu artıq validator-da olduğu üçün buradan sildik.
        // (İstəyə görə saxlanıla bilər, amma təkrar kodu aradan qaldırmaq üçün sildim)
    }
}