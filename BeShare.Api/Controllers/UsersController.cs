using BeShare.Api.Data;
using BeShare.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.Username,
                    IsAdmin = u.IsAdmin
                })
                .ToListAsync();

            return users;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return users;
        }

        public class UserDto
        {
            public int Id { get; set; }
            public string Email { get; set; }
            public string Username { get; set; }
            public bool IsAdmin { get; set; }
        }
    }
}
