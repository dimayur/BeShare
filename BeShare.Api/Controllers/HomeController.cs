using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeShare.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BeShare.Api.Models;

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

            var fileName = Guid.NewGuid().ToString();
            var fileType = Path.GetExtension(file.FileName);
            var fullFileName = fileName + fileType;
            var filePath = Path.Combine("userfiles", fullFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            string formattedDate = DateTime.Now.ToString("dd.MM.yyyy");

            var uploadedFile = new UploadedFile
            {
                UserId = user.Id,
                FileName = fullFileName,
                FileType = fileType,
                UploadDate = formattedDate
            };

            _context.UploadedFiles.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return Ok("Файл успішно завантажено");
        }
        [HttpPost("api/getfiles")]
        public async Task<IActionResult> GetFilesForUser(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return Unauthorized("Недійсний токен доступу");

            var files = await _context.UploadedFiles
                .Where(f => f.UserId == user.Id)
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.FileType,
                    f.UploadDate
                })
                .ToListAsync();

            return Ok(files);
        }

    }
}
