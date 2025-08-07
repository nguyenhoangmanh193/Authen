using System.ComponentModel.DataAnnotations;

namespace JwtAuth.Models
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required.")]
        [MinLength(3, ErrorMessage = "Current password must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "Current password cannot exceed 100 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(3, ErrorMessage = "New password must be at least 3 characters.")]
        [MaxLength(100, ErrorMessage = "New password cannot exceed 100 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
