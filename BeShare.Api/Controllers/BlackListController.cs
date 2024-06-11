using BeShare.Api.Data;
using BeShare.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlackListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtSecret;
        
        public BlackListController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _jwtSecret = configuration["Jwt:Secret"];
        }
        [HttpPost("api/blacklist/add")]
        public async Task<IActionResult> AddToBlacklist(string token, string md5Hash, string reason)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null || !user.IsAdmin)
                return Unauthorized("Недійсний токен доступу або у вас немає прав адміністратора");

            if (string.IsNullOrEmpty(md5Hash))
                return BadRequest("MD5 хеш не може бути порожнім");

            if (await IsFileBlacklisted(md5Hash))
                return BadRequest("Цей файл вже знаходиться у чорному списку");

            if (string.IsNullOrEmpty(reason))
                return BadRequest("Причина не може бути порожньою");

            var blacklistedFile = new BlackList
            {
                MD5Hash = md5Hash,
                Reason = reason
            };

            _context.BlacklistedFiles.Add(blacklistedFile);
            await _context.SaveChangesAsync();

            return Ok("Файл успішно додано до чорного списку");
        }

        [HttpPost("api/blacklist/delete")]
        public async Task<IActionResult> RemoveFromBlacklist(string token, int fileId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null || !user.IsAdmin)
                return Unauthorized("Недійсний токен доступу або у вас немає прав адміністратора");

            var blacklistedFile = await _context.BlacklistedFiles.FindAsync(fileId);
            if (blacklistedFile == null)
                return NotFound("Файл не знайдено в чорному списку");

            _context.BlacklistedFiles.Remove(blacklistedFile);
            await _context.SaveChangesAsync();

            return Ok("Файл успішно видалено з чорного списку");
        }

        [HttpGet("api/blacklist/get")]
        public async Task<IActionResult> GetBlacklistedFiles(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Token == token);
            if (user == null || !user.IsAdmin)
                return Unauthorized("Недійсний токен доступу або у вас немає прав адміністратора");

            var blacklistedFiles = await _context.BlacklistedFiles.Select(bf => new { bf.Id, bf.MD5Hash, bf.Reason }).ToListAsync();

            return Ok(blacklistedFiles);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<bool> IsFileBlacklisted(string md5Hash)
        {
            return await _context.BlacklistedFiles.AnyAsync(bf => bf.MD5Hash == md5Hash);
        }
    }
}
