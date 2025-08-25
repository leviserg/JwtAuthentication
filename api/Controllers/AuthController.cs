using JwtAuthentication.Data.Models.Auth;
using JwtAuthentication.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthUserService authUserService) : ControllerBase
    {

        public static User user = new();


        [HttpPost("register")]
        public ActionResult<User> Register(UserDto registerRequest)
        {

            var hashedPassword = new PasswordHasher<User>().HashPassword(user, registerRequest.Password);

            user.Name = registerRequest.Name;
            user.PasswordHash = hashedPassword;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDto loginRequest)
        {
            if (!user.Name.Equals(loginRequest.Name))
                return BadRequest("User not found");

            var passwordVerification = new PasswordHasher<User>().VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginRequest.Password
            );

            if (passwordVerification == PasswordVerificationResult.Failed)
                return BadRequest("Wrong password");

            var token = authUserService.CreateToken(user);

            return Ok(token);
        }


    }
}
