using JwtAuthentication.Data.Models.Auth;
using JwtAuthentication.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthUserService authUserService) : ControllerBase
    {

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(UserDto request)
        {

            var result = await authUserService.RegisterAsync(request);

            if (result.HasErrors)
                return BadRequest(string.Join(';', result.Errors));

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AccessToken>> Login(UserDto request)
        {

            var result = await authUserService.LoginAsync(request);

            if (result.HasErrors)
                return BadRequest(string.Join(';', result.Errors));

            return Ok(result.Value);
        }


    }
}
