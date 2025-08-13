using JwtAuth.Entities;
using JwtAuth.Models;
using JwtAuth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JwtAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController(IAuthService authService) : ControllerBase
    {
        public static User user = new();

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserDto request)
        {

            var user = await authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("User already exists");
            }
            return Ok(user);
        }
        [HttpGet("test")]
        public async Task<ActionResult<string>> Test()
        {
            // This is just a test endpoint to verify the API is working
            return Ok("API is working");
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto request)
        {
            try
            {
                var result = await authService.LoginAsync(request);
                if (result == null)
                {
                    return BadRequest("Invalid credentials");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if ( userIdClaim == null || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized("Invalid token");
            }

            var result = await authService.LogoutAsync(userId);
            if (!result)
            {
                return NotFound("User not found");
            }

            return Ok("Logged out successfully");
        }

        [Authorize]
        [HttpPut("/{id}/password")]
        public async Task<IActionResult> ChangePassword(Guid id, ChangePasswordDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || userIdClaim != id.ToString())
            {
                return Forbid("You can only change your own password.");
            }

            var result = await authService.ChangePasswordAsync(id, dto);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => e.Description));
            }

            return Ok("Password changed successfully.");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("list-users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
            {
              if (page <= 0) page = 1;
              if (pageSize <= 0) pageSize = 10;

            var query = authService.QueryUsersByRole("User");

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Username.Contains(search));
            }

            var users = await query
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(users);
        }

        [Authorize]
        [HttpDelete("/delete-account")] 
        public async Task<IActionResult> DeleteMyAccount()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { Message = "Invalid token" });

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { Message = "Invalid user ID in token" });

            var result = await authService.DeleteUserAsync(userId);
            if (!result)
                return NotFound(new { Message = "User not found" });

            return NoContent(); 
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost("lock-user")]
        public async Task<IActionResult> BulkLockUsers([FromBody] LockUsersRequestDto request)
        {
            if (request.UserIds == null || request.UserIds.Count == 0)
                return BadRequest("UserIds list is empty.");

            var result = await authService.LockUsersAsync(request.UserIds);

            if (!result)
                return NotFound("No users found to lock.");

            return Ok("Users locked successfully.");
        }






        [Authorize]
        [HttpGet]
        public IActionResult AuthenticationOnlyEndpoint()
        {
            return Ok("This endpoint is Authited");
        }


        [Authorize(Roles ="Admin")]
        [HttpGet("admin")]
        public IActionResult AdminOnlyEndpoint()
        {
              return Ok("This endpoint is Admin");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(result);
        }




    }
}
