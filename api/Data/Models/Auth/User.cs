using System.ComponentModel.DataAnnotations;

namespace JwtAuthentication.Data.Models.Auth
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
