using JwtAuthentication.Data.Models.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthentication.Services.Auth
{

    public interface IAuthUserService
    {
        public string CreateToken(User user);
    }
    public class AuthUserService(IConfiguration configuration) : IAuthUserService
    {
        public string CreateToken(User user)
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

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
