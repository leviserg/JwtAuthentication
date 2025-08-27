using JwtAuthentication.Data;
using JwtAuthentication.Data.Models;
using JwtAuthentication.Data.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthentication.Services.Auth
{

    public interface IAuthUserService
    {
        Task<Required<User, string>> RegisterAsync(UserDto request);

        Task<Required<AccessToken, string>> LoginAsync(UserDto request);

    }
    public class AuthUserService(
        IApplicationDbContext applicationDbContext,
        IConfiguration configuration) : IAuthUserService
    {

        public async Task<Required<User, string>> RegisterAsync(UserDto request)
        {





            var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);

            user.Name = request.Name;
            user.PasswordHash = hashedPassword;
            // Here you would typically save the user to a database
            // For this example, we'll just return the user object
            return await Task.FromResult(user);
        }


        public async Task<Required<AccessToken, string>> LoginAsync(UserDto request)
        {
            var user = new User
            {
                Name = request.Name,
                PasswordHash = new PasswordHasher<User>().HashPassword(null!, request.Password)
            };
            // Here you would typically save the user to a database
            // For this example, we'll just return the user object
            return await Task.FromResult(user);
        }


        private AccessToken CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
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
                ExpirationSeconds = (int)TimeSpan.FromHours(1).TotalSeconds
            };
        }
    }
}
