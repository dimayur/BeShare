using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BeShare.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using BeShare.Api.Models;
using Microsoft.VisualBasic.FileIO;

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
        [HttpPost("api/user/validuser")]
        public async Task<IActionResult> GetUserName(string token)
        {
            if (string.IsNullOrEmpty(token))
                return BadRequest("Токен не отримано");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return NotFound("Користувача не знайдено");

            return Ok(new { UserName = user.Username });
        }
        [HttpPost("api/upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string token)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не вибрано");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Токен відсутній.");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return Unauthorized("Недійсний токен доступу");

            string md5Hash;
            string originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
            string fileExtension = Path.GetExtension(file.FileName);
            string uniqueFileName = $"{originalFileName}{fileExtension}";
            string filePath = Path.Combine("userfiles", uniqueFileName);

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                md5Hash = CalculateMD5Hash(memoryStream);
                memoryStream.Position = 0;
                if (await IsFileBlacklisted(md5Hash))
                    return BadRequest("Цей файл заборонено завантажувати");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    memoryStream.CopyTo(stream);
                }
            }

            string formattedDate = DateTime.Now.ToString("dd.MM.yyyy");
            var uploadedFile = new UploadedFile
            {
                UserId = user.Id,
                FileName = uniqueFileName,
                FileType = fileExtension,
                UploadDate = formattedDate,
                MD5Hash = md5Hash
            };

            _context.UploadedFiles.Add(uploadedFile);
            await _context.SaveChangesAsync();

            return Ok("Файл успішно завантажено");
        }
        [HttpPost("getfiles")]
        public async Task<IActionResult> GetUserFiles(string token)
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
        

        [HttpDelete("api/deletefile/{id}")]
        public async Task<IActionResult> DeleteFile(string token, int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null)
                return Unauthorized("Недійсний токен доступу");
            var file = await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id && f.UserId == user.Id);
            if (file == null)
                return NotFound("Файл не знайдено");
            _context.UploadedFiles.Remove(file);
            await _context.SaveChangesAsync();
            return Ok("Файл успішно видалено");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<bool> IsFileBlacklisted(string md5Hash)
        {
            return await _context.BlacklistedFiles.AnyAsync(bf => bf.MD5Hash == md5Hash);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public string CalculateMD5Hash(Stream inputStream)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(inputStream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
