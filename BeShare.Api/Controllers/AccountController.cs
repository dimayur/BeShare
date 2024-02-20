using BeShare.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BeShare.Api.Models;

namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["Jwt:Secret"];
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            // Перевірка унікальності імені користувача
            if (await _context.Users.AnyAsync(x => x.Username == model.Username))
            {
                return Conflict("Це ім'я користувача вже зайняте");
            }

            // Хешування пароля
            var passwordHash = HashPassword(model.Password);

            var user = new User
            {
                Username = model.Username,
                PasswordHash = passwordHash,
                Email = model.Email
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Користувач успішно зареєстрований");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username == model.Username);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Невірне ім'я користувача або пароль");
            }

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string enteredPassword, string savedPasswordHash)
        {
            var hashedEnteredPassword = HashPassword(enteredPassword);
            return savedPasswordHash.Equals(hashedEnteredPassword);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddHours(4), // ПОМІНЯЙ ВСЕ НАЗАД 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
