using CharitySystem.Application.DTOs.Auth;
using CharitySystem.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using CharitySystem.Application.Interfaces;
using CharitySystem.Domain.Entities;
using CharitySystem.Domain.Enums;



namespace CharitySystem.Application.Services
{
    public class AuthService : IAuthService 
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto request)
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.Username == request.Username);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                throw new Exception("Username or passwdrd is incorrect");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new Exception("Username or passwdrd is incorrect");
            }

            if (user.AccountStatus == AccountStatus.Inactive)
            {
                throw new Exception("This account is Inactive, please contact the charity.");
            }

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserID,
                Username = user.Username,
                Role = user.Role.ToString(),
                Token = token,
                Expiration = DateTime.Now.AddDays(7) 
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(double.Parse(jwtSettings["DurationInHours"])),
                SigningCredentials = creds,
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
