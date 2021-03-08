using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System;

namespace UsersCanLogIn.API.Controllers
{
    [ApiVersion("1"), Route("password-reset-tokens")]
    [ApiController]
    [Produces("application/json")]
    public class PasswordResetTokensController : ControllerBase
    {
        private const string BadRequestErrorTitle = "Invalid Password Reset Token Request";
        private const string BadRequestEmailErrorDetail = "A user with the given email address was not found.";
        private const string BadRequestUsernameErrorDetail = "A user with the given username was not found.";
        private const string NotFoundErrorTitle = "Password Reset Token Not Found";
        private const string NotFoundErrorDetail = "Password reset token is invalid or expired.";

        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
        private readonly IResponseObjectFactory _responseObjectFactory;
        private readonly IUserRepository _userRepository;

        public PasswordResetTokensController(IPasswordResetTokenRepository passwordResetTokenRepository, IResponseObjectFactory responseObjectFactory,
            IUserRepository userRepository)
        {
            _passwordResetTokenRepository = passwordResetTokenRepository;
            _responseObjectFactory = responseObjectFactory;
            _userRepository = userRepository;
        }

        // GET: api/v1/password-reset-tokens/abc
        [HttpGet("{id}")]
        public async Task<ActionResult<IResponseObject>> GetPasswordResetToken(string id)
        {
            PasswordResetToken token = await _passwordResetTokenRepository.GetById(id);
            if (token == null)
            {
                IResponseObject errorResponse = _responseObjectFactory
                    .CreateErrorResponseObject(HttpStatusCode.NotFound, NotFoundErrorTitle, NotFoundErrorDetail);
                return NotFound(errorResponse);
            }
            return _responseObjectFactory.CreateResponseObject(new { token.Id });
        }

        // POST: api/v1/password-reset-tokens
        [HttpPost]
        public async Task<ActionResult<IResponseObject>> PostPasswordResetToken(PasswordResetTokenRequestDTO requestDTO)
        {
            bool useEmail = !string.IsNullOrEmpty(requestDTO.Email);
            User user = useEmail ? await _userRepository.GetByEmail(requestDTO.Email.ToLower()) :
                await _userRepository.GetByUsername(requestDTO.Username.ToLower());
            if (user == null)
            {
                string errorDetail = useEmail ? BadRequestEmailErrorDetail : BadRequestUsernameErrorDetail;
                IResponseObject errorResponse = _responseObjectFactory
                    .CreateErrorResponseObject(HttpStatusCode.BadRequest, BadRequestErrorTitle, errorDetail);
                return BadRequest(errorResponse);
            }
            string siteUrl = string.IsNullOrEmpty(requestDTO.SiteUrlOverride) ?
                $"{HttpContext.Request.Scheme}{Uri.SchemeDelimiter}{HttpContext.Request.Host}" : requestDTO.SiteUrlOverride;
            await _passwordResetTokenRepository.CreateDefaultAndEmail(user.Email, user.Username, siteUrl);
            return NoContent();
        }
    }
}
