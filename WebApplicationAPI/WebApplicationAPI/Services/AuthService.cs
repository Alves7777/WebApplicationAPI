using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Verificar se email já existe
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email já cadastrado");
            }

            // Hash da senha usando BCrypt
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var userId = await _userRepository.CreateAsync(user);
            user.Id = userId;

            var (token, expiresAt) = GenerateJwtToken(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email ou senha inválidos");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Usuário inativo");
            }

            var (token, expiresAt) = GenerateJwtToken(user);

            return new AuthResponse
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Token = token,
                Role = user.Role,
                ExpiresAt = expiresAt
            };
        }

        private (string token, DateTime expiresAt) GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? "ChaveSecretaSuperSegura123!@#MinimoDe32Caracteres";
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "WebApplicationAPI";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "WebApplicationAPI";
            var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationInMinutes", 60);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
        }
    }
}
