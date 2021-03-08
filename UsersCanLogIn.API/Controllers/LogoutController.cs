using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.Controllers
{
    [ApiVersion("1"), Route("logout")]
    [ApiController]
    [Produces("application/json")]
    public class LogoutController : ControllerBase
    {
        // GET: api/v1/logout
        [HttpPost]
        public async Task<IActionResult> PostLogout()
        {
            await HttpContext.SignOutAsync();
            return NoContent();
        }
    }
}
