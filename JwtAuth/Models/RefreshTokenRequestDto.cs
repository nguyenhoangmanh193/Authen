using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Models
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "UserId is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "RefreshToken is required.")]
        [MaxLength(256, ErrorMessage = "RefreshToken cannot exceed 256 characters.")]
        public string RefreshTokenHash { get; set; } = string.Empty;
    }
}
