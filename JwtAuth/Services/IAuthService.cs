using JwtAuth.Entities;
using JwtAuth.Models;
using Microsoft.AspNetCore.Identity;

namespace JwtAuth.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto refresh );
        Task<bool> LogoutAsync(Guid userId);
        Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
