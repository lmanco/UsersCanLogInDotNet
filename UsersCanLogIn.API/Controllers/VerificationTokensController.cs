using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.Controllers
{
    [ApiVersion("1"), Route("verification-tokens")]
    [ApiController]
    [Produces("application/json")]
    public class VerificationTokensController : ControllerBase
    {
        private const string BadRequestErrorTitle = "Invalid Verification Token Request";
        private const string BadRequestEmailErrorDetail = "A user with the given email address was not found.";
        private const string BadRequestUsernameErrorDetail = "A user with the given username was not found.";

        private readonly IVerificationTokenRepository _verificationTokenRepository;
        private readonly IResponseObjectFactory _responseObjectFactory;
        private readonly IUserRepository _userRepository;

        public VerificationTokensController(IVerificationTokenRepository verificationTokenRepository, IResponseObjectFactory responseObjectFactory,
            IUserRepository userRepository)
        {
            _verificationTokenRepository = verificationTokenRepository;
            _responseObjectFactory = responseObjectFactory;
            _userRepository = userRepository;
        }

        // POST: api/v1/verification-tokens
        [HttpPost]
        public async Task<ActionResult<IResponseObject>> PostVerificationToken(VerificationTokenRequestDTO requestDTO)
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
            await _verificationTokenRepository.CreateDefaultAndEmail(user.Email, user.Username, siteUrl);
            return NoContent();
        }
    }
}
