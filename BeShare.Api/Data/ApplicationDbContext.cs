using BeShare.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BeShare.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<BlackList> BlacklistedFiles { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
