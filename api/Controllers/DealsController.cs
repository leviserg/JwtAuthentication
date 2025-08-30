using JwtAuthentication.Data.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace JwtAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DealsController : ControllerBase
    {


        private const string AllowedAdvancedRoles =$"{UserRole.Admin},{UserRole.AdvancedUser}";

        [Authorize]
        [HttpGet("public")]
        public IActionResult GetDeals()
        {
            var deals = new[]
            {
                new { Id = 1, Name = "Deal 1", Description = "Description for Deal 1" },
                new { Id = 2, Name = "Deal 2", Description = "Description for Deal 2" },
                new { Id = 3, Name = "Deal 3", Description = "Description for Deal 3" }
            };
            return Ok(deals);
        }

        [Authorize(Roles = AllowedAdvancedRoles)]
        [HttpGet("secret")]
        public IActionResult GetSecretDeals()
        {
            var deals = new[]
            {
                new { Id = 1, Name = "Secret Deal 111", Description = "Super Secret Deal Project 111" },
                new { Id = 2, Name = "Secret Deal 2222", Description = "Higly classified Deal Project 222" },
            };
            return Ok(deals);
        }
    }
}
