using AuthECBackend.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthECBackend.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        // This is neccessary, because we are using IdentityDbContext
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
