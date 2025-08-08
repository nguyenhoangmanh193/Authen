using JwtAuth.Data;
using JwtAuth.Entities;
using JwtAuth.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EFCore.BulkExtensions;

namespace JwtAuth.Services
{
    public class AuthService(UserDbContext context, IConfiguration configuration
       ) : IAuthService
    {
        
        public async Task<TokenResponseDto?> LoginAsync(UserDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return null; // User not found
            }

            // Check if account is locked
            if (user.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                // Account is temporarily locked
                throw new Exception($"Account is locked until {user.LockoutEnd.Value:u}");
            }

            if (new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null; // Invalid password
            }

            var token = await CreateTokenResponse(user);

            return token; 

        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateSaveRefreshToken(user)
            };
        }

        public async Task<User?> RegisterAsync(UserDto request)
        {
            if (await context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return null; // User already exists
            }

            var user = new User();
            var hashedPassWord = new PasswordHasher<User>()
               .HashPassword(user, request.Password);

            user.Username = request.Username;
            user.PasswordHash = hashedPassWord;

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> LogoutAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null)
            {
                return false; // User not found
            }
            user.RefreshTokenHash = null;
            user.RefreshTokenTimeExpire = null;

            await context.SaveChangesAsync();
            return true; // Logout successful
        }

        public async Task<IdentityResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            
            var passwordHasher = new PasswordHasher<User>();
            var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Current password is incorrect" });
            }

           
            user.PasswordHash = passwordHasher.HashPassword(user, dto.NewPassword);

           
            await context.SaveChangesAsync();

            return IdentityResult.Success;
        }


        public async Task<List<ListUserResponseDto>> GetAllUsersAsync()
        {
            return await context.Users
                .Where(u => u.Role == "User")
                .Select(u => new ListUserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,                
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LockUsersAsync(List<Guid> userIds)
        {
            //var users = await context.Users
            //    .Where(u => userIds.Contains(u.Id))
            //    .ToListAsync();

            //if (users.Count == 0)
            //    return false;

            //foreach (var user in users)
            //{
            //    user.IsLocked = true;
            //    user.LockoutEnd = DateTime.UtcNow.AddDays(7); // Lock 7 day
            //}

            //await context.SaveChangesAsync();
            var usersToUpdate = userIds.Select(id => new User
            {
                Id = id,
                IsLocked = true,
                LockoutEnd = DateTime.UtcNow.AddDays(7)
            }).ToList();

            await context.BulkUpdateAsync(usersToUpdate);

            return true;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)); // nên lưu trong cấu hình

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null ||   user.RefreshTokenTimeExpire <= DateTime.UtcNow)
            {
                return null;
            }

            var refreshTokenHash = HashToken(refreshToken);

            if (user.RefreshTokenHash != refreshTokenHash      )
            {
                return null;
            }

            return user;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string HashToken(string token)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private async Task<string?> GenerateSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();

            var refreshTokenHash = HashToken(refreshToken);

            user.RefreshTokenHash = refreshTokenHash;
            user.RefreshTokenTimeExpire = DateTime.UtcNow.AddHours(1); // Set expiration for 7 days
            await context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshTokenHash);
            if (user is null)
            {
                return null;
            }

            return await CreateTokenResponse(user);

        }

        public IQueryable<ListUserResponseDto> QueryUsersByRole(string role = "User")
        {
            return context.Users
                .Where(u => u.Role == role)
                .Select(u => new ListUserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username
                });
        }


    }

}
