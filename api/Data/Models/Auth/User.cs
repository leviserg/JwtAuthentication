using System.ComponentModel.DataAnnotations;

namespace JwtAuthentication.Data.Models.Auth
{
    public class User
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public required string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; } = null;
    }
}
