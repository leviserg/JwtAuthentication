using System.ComponentModel.DataAnnotations;

namespace JwtAuthentication.Data.Models.Auth
{
    public class UserDto
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(100)]
        public required string Password { get; set; }
        
        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; } = null;

        [MaxLength(100)]
        public string? Role { get; set; }
    }
}
