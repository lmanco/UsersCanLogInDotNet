using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.DAL.Repositories;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net;
using System.ComponentModel.DataAnnotations;
using UsersCanLogIn.API.Util;

namespace UsersCanLogIn.API.Controllers
{
    [ApiVersion("1"), Route("login")]
    [ApiController]
    [Produces("application/json")]
    public class LoginController : ControllerBase
    {
        private const string InvalidLoginErrorTitle = "Invalid Login";
        private const string ErrorDetailEmail = "Email or password is invalid.";
        private const string ErrorDetailUsername = "Username or password is invalid.";
        private const string VerificationErrorTitle = "Account Not Verified";
        private const string ErrorDetailVerification = "Account is not yet verified.";

        private readonly IUserRepository _userRepository;
        private readonly IResponseObjectFactory _responseObjectFactory;
        private readonly IPasswordHasher _passwordHasher;

        public LoginController(IUserRepository userRepository, IResponseObjectFactory responseObjectFactory, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _responseObjectFactory = responseObjectFactory;
            _passwordHasher = passwordHasher;
        }

        // POST: api/v1/login
        [HttpPost]
        public async Task<ActionResult<IResponseObject>> PostLogin(LoginRequestDTO loginDTO)
        {
            bool useEmail = !string.IsNullOrEmpty(loginDTO.Email);
            User user = useEmail ? await _userRepository.GetByEmail(loginDTO.Email.ToLower()) :
                await _userRepository.GetByUsername(loginDTO.Username.ToLower());
            if (user == null || !_passwordHasher.Verify(loginDTO.Password, user.PasswordHash))
                return LoginError(InvalidLoginErrorTitle, useEmail ? ErrorDetailEmail : ErrorDetailUsername);
            if (!user.Verified)
                return LoginError(VerificationErrorTitle, ErrorDetailVerification);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return NoContent();
        }

        private ActionResult<IResponseObject> LoginError(string errorTitle, string errorDetail)
        {
            ErrorResponseObject errorResponse = _responseObjectFactory
                .CreateErrorResponseObject(HttpStatusCode.BadRequest, errorTitle, errorDetail);
            return BadRequest(errorResponse);
        }
    }

    public class LoginRequestDTO
    {
        [RequiredIfMissing("Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
