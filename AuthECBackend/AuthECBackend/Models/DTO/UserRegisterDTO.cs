using System.ComponentModel.DataAnnotations;

namespace AuthECBackend.Models.DTO
{
    public class UserRegisterDTO
    {
        public required string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }

        public required string Gender { get; set; }
        public int? LibraryId { get; set; }
        public int Age { get; set; }
        public required string Role { get; set; }
    }
}
