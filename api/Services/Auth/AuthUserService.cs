using JwtAuthentication.Data;
using JwtAuthentication.Data.Models;
using JwtAuthentication.Data.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthentication.Services.Auth
{

    public interface IAuthUserService
    {
        Task<Required<Guid, string>> RegisterAsync(UserDto request);

        Task<Required<TokenResponseDto, string>> LoginAsync(UserDto request);

        Task<Required<TokenResponseDto, string>> RefreshTokensAsync(RefreshTokenRequestDto request);

    }
    public class AuthUserService(
        IApplicationDbContext dbContext,
        IConfiguration configuration) : IAuthUserService
    {

        private const int TokenExpirationHours = 1;
        private const int RefreshTokenExpirationDays = 7;
        
        #region Public methods
        public async Task<Required<Guid, string>> RegisterAsync(UserDto request)
        {

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new Required<Guid, string>(Guid.Empty, "Email required");
            }

            if(!string.IsNullOrEmpty(request.Role))
            {
                var availableRoles = new List<string> { UserRole.Admin, UserRole.AdvancedUser, UserRole.User };
                if (!availableRoles.Contains(request.Role))
                    return new Required<Guid, string>(Guid.Empty, "Invalid role");
            }

            if (await dbContext.Users.AnyAsync(u => u.Name == request.Name))
            {
                return new Required<Guid, string>(Guid.Empty, $"User with the name {request.Name} already exists");
            }

            var user = new User() { 
                Name = request.Name,
                Email = request.Email,
                Role = request.Role
            };

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            user.PasswordHash = hashedPassword;

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            return new Required<Guid, string>(user.Id);
        }


        public async Task<Required<TokenResponseDto, string>> LoginAsync(UserDto request)
        {

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);

            if (user == null)
                return new Required<TokenResponseDto, string>(new TokenResponseDto(), "User not found");

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (passwordVerification == PasswordVerificationResult.Failed)
                return new Required<TokenResponseDto, string>(new TokenResponseDto(), "Wrong password");

            var tokenResponse = await CreateTokenResponseAsync(user);

            return new Required<TokenResponseDto, string>(tokenResponse);
        }

        public async Task<Required<TokenResponseDto, string>> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request);
            if (user is null)
            {
                return new Required<TokenResponseDto, string>(new TokenResponseDto(), "Invalid refresh token or user");
            }

            var tokenResponse = await CreateTokenResponseAsync(user);

            return new Required<TokenResponseDto, string>(tokenResponse);
        }

        #endregion


        #region Private methods


        private async Task<TokenResponseDto> CreateTokenResponseAsync(User user)
        {
            var refreshToken = await GetOrAddRefreshToken(user);

            var tokenResponse = CreateToken(user);
            tokenResponse.RefreshToken = refreshToken;
            return tokenResponse;
        }


        private TokenResponseDto CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? UserRole.User)
            };

            var appSettingsKey = configuration.GetValue<string>("AuthSettings:TokenKey");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(appSettingsKey!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AuthSettings:Issuer"),
                audience: configuration.GetValue<string>("AuthSettings:Audience"),
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddHours(1),
                claims: claims
            );

            return new TokenResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor),
                ExpirationSeconds = (int)TimeSpan.FromHours(TokenExpirationHours).TotalSeconds
            };
        }

        private async Task<string> GetOrAddRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);
            await dbContext.SaveChangesAsync();
            return refreshToken;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<User?> ValidateRefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await dbContext.Users.FindAsync(request.UserId);
            if (user == null 
                || user.RefreshToken != request.RefreshToken
                || user.RefreshTokenExpiration <= DateTime.UtcNow)
                return null;
            return user;
        }

        #endregion
    }
}
