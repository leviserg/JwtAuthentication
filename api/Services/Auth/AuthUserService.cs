using JwtAuthentication.Data;
using JwtAuthentication.Data.Models;
using JwtAuthentication.Data.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthentication.Services.Auth
{

    public interface IAuthUserService
    {
        Task<Required<UserDto, string>> RegisterAsync(UserDto request);

        Task<Required<AccessToken, string>> LoginAsync(UserDto request);

    }
    public class AuthUserService(
        IApplicationDbContext dbContext,
        IConfiguration configuration) : IAuthUserService
    {

        private const int TokenExpirationHours = 1;

        public async Task<Required<UserDto, string>> RegisterAsync(UserDto request)
        {

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new Required<UserDto, string>(request, "Email required");
            }

            if(!string.IsNullOrEmpty(request.Role))
            {
                var availableRoles = new List<string> { UserRole.Admin, UserRole.AdvancedUser, UserRole.User };
                if (!availableRoles.Contains(request.Role))
                    return new Required<UserDto, string>(request, "Invalid role");
            }

            if (await dbContext.Users.AnyAsync(u => u.Name == request.Name))
            {
                return new Required<UserDto, string>(request, $"User with the name {request.Name} already exists");
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

            return new Required<UserDto, string>(request);
        }


        public async Task<Required<AccessToken, string>> LoginAsync(UserDto request)
        {

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Name);

            if (user == null)
                return new Required<AccessToken, string>(new AccessToken(), "User not found");

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (passwordVerification == PasswordVerificationResult.Failed)
                return new Required<AccessToken, string>(new AccessToken(), "Wrong password");

            return new Required<AccessToken, string>(CreateToken(user));
        }


        private AccessToken CreateToken(User user)
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

            return new AccessToken
            {
                Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor),
                ExpirationSeconds = (int)TimeSpan.FromHours(TokenExpirationHours).TotalSeconds
            };
        }
    }
}
