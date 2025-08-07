using Microsoft.AspNetCore.Identity;

namespace JwtAuth.Entities
{
    public class User 
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } =  "User";
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenTimeExpire { get; set; }
    }
}
