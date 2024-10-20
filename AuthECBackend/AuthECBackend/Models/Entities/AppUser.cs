using Microsoft.AspNetCore.Identity;

namespace AuthECBackend.Models.Entities
{
    public class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
