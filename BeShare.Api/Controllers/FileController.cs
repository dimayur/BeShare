using BeShare.Api.Data;
using BeShare.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.FileProviders;


namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase  
    {
        private readonly ApplicationDbContext _context;
        private readonly string _fileDirectory;

        public FileController(ApplicationDbContext context)
        {
            _context = context;
            _fileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "userfiles"); 
            if (!Directory.Exists(_fileDirectory)) 
            {
                Directory.CreateDirectory(_fileDirectory);
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(int id)
        {
            var file = await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
                return NotFound("Файл не знайдено");

            if (await IsFileBlacklisted(file.MD5Hash))
                return BadRequest("Цей файл заборонено завантажувати");

            var filePath = Path.Combine(_fileDirectory, file.FileName);
            if (!System.IO.File.Exists(filePath))
                return NotFound("Файл не знайдено");
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            memory.Position = 0;
            return File(memory, "application/octet-stream", Path.GetFileName(filePath)); // Спорно но ок
        }
        [HttpGet("download/info/{id}")]
        public async Task<IActionResult> GetFileById(int id)
        {
            var file = await _context.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
                return NotFound("Файл не знайдено");

            var fileInfo = new
            {
                file.Id,
                file.FileName,
                file.FileType,
                file.UploadDate,
                file.MD5Hash
            };

            return Ok(fileInfo);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        public async Task<bool> IsFileBlacklisted(string md5Hash)
        {
            return await _context.BlacklistedFiles.AnyAsync(bf => bf.MD5Hash == md5Hash);
        }
    }
}   
    
