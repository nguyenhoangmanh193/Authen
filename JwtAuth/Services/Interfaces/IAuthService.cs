using JwtAuth.Entities;
using JwtAuth.Models;
using Microsoft.AspNetCore.Identity;

namespace JwtAuth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto refresh );
        Task<bool> LogoutAsync(Guid userId);
        Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
        IQueryable<ListUserResponseDto> QueryUsersByRole(string role = "User");
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> LockUsersAsync(List<Guid> userIds);



        // 
    }
}
