using BeShare.Api.Data;
using BeShare.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace BeShare.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : Controller
    {
        private readonly ApplicationDbContext _context;
        public FileController(ApplicationDbContext context)
        {
            _context = context;
        }
    }
}   
    
