using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeShare.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("api/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string token)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не вибрано");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return Unauthorized("Недійсний токен доступу");

            var fileName = Guid.NewGuid().ToString(); // Назва
            var fileType = Path.GetExtension(file.FileName); // Тип файлу
            var fullFileName = fileName + fileType;
            var filePath = Path.Combine("userfiles", fullFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);            }

            return Ok("Файл успішно завантажено");
        }
    }
}
