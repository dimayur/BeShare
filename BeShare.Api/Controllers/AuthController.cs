using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeShare.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<string> { "User1", "User2", "User3" };
            return Ok(users);
        }
    }
}
