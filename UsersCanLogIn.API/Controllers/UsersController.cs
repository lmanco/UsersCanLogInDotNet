using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.DAL.Repositories;
using System.Collections.Generic;
using AutoMapper;
using System.Net;
using System.Security.Claims;
using UsersCanLogIn.API.Controllers.Filters;
using UsersCanLogIn.API.Util;
using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace UsersCanLogIn.Controllers
{
    [ApiVersion("1"), Route("users")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private const string UserCreationErrorTitle = "Invalid New User";
        private const string EmailInUseErrorDetail = "The email address provided is already in use.";
        private const string UsernameTakenErrorDetail = "The username provided is already taken.";
        private const string UpdateForbiddenTitle = "Forbidden";
        private const string UpdateForbiddenDetail = "You do not have permission to modify this user.";
        private const string UserUpdateErrorTitle = "Invalid User Update";
        private const string UserUpdatePasswordErrorDetail = "Password is incorrect.";
        private const string UserUpdateVerificationErrorDetail = "Verification token is invalid or expired.";
        private const string UserUpdateVerificationUserErrorDetail = "The user for the specified verification token does not exist.";
        private const string UserUpdatePasswordResetErrorDetail = "Password reset token is invalid or expired.";
        private const string UserUpdatePasswordResetUserErrorDetail = "The user for the specified password reset token does not exist.";
        private const string NotFoundTitle = "Not Found";
        private const string NotFoundDetail = "The requested user was not found.";

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IResponseObjectFactory _responseObjectFactory;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IVerificationTokenRepository _verificationTokenRepository;
        private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;

        public UsersController(IUserRepository userRepository, IMapper mapper, IResponseObjectFactory responseObjectFactory, IPasswordHasher passwordHasher,
            IVerificationTokenRepository verificationTokenRepository, IPasswordResetTokenRepository passwordResetTokenRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _responseObjectFactory = responseObjectFactory;
            _passwordHasher = passwordHasher;
            _verificationTokenRepository = verificationTokenRepository;
            _passwordResetTokenRepository = passwordResetTokenRepository;
        }

        // GET: api/v1/users
        [HttpGet]
        [RestrictByRoles(new UserRole[] { UserRole.Admin })]
        public async Task<ActionResult<IResponseObject>> GetUsers()
        {
            IEnumerable<User> users = await _userRepository.List();
            IEnumerable<UserResponseDTO> userDTOs = _mapper.Map<IEnumerable<UserResponseDTO>>(users);
            return _responseObjectFactory.CreateResponseObject(userDTOs);
        }

        // GET: api/v1/users/5
        [HttpGet("{id}")]
        [RestrictByRoles(new UserRole[] { UserRole.Admin })]
        public async Task<ActionResult<IResponseObject>> GetUser(long id)
        {
            User user = await _userRepository.GetById(id);
            if (user == null)
                return UserNotFound();
            UserResponseDTO userDTO = _mapper.Map<UserResponseDTO>(user);
            return _responseObjectFactory.CreateResponseObject(userDTO);
        }

        // GET: api/v1/users/self
        [HttpGet("self")]
        public async Task<ActionResult<IResponseObject>> GetUser()
        {
            int currentUserId = int.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            User user = await _userRepository.GetById(currentUserId);
            if (user == null)
                return UserNotFound();
            UserResponseDTO userDTO = _mapper.Map<UserResponseDTO>(user);
            return _responseObjectFactory.CreateResponseObject(userDTO);
        }

        // PATCH: api/v1/users/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<IResponseObject>> PatchUser(long id, UserUpdateRequestDTO userUpdateDTO)
        {
            if (!IsSelfOrAdmin(id))
                return Forbidden();
            User user = await _userRepository.GetById(id);
            if (user == null)
                return UserNotFound();
            if (!_passwordHasher.Verify(userUpdateDTO.Password, user.PasswordHash))
                return BadRequest(UserUpdateErrorTitle, UserUpdatePasswordErrorDetail);
            if (!string.IsNullOrEmpty(userUpdateDTO.Username))
                user.Username = userUpdateDTO.Username;
            return await TryPerformUserUpdate(user);
        }

        // PATCH: api/v1/users/password
        [HttpPatch("password")]
        public async Task<ActionResult<IResponseObject>> PatchUserPassword(UserPasswordResetUpdateDTO passwordResetDTO)
        {
            PasswordResetToken token = await _passwordResetTokenRepository.GetById(passwordResetDTO.PasswordResetTokenId);
            if (token == null || DateTime.Now >= token.Expiration)
                return BadRequest(UserUpdateErrorTitle, UserUpdatePasswordResetErrorDetail);
            User user = await _userRepository.GetByEmail(token.Email);
            if (user == null)
                return BadRequest(UserUpdateErrorTitle, UserUpdatePasswordResetUserErrorDetail);
            user.PasswordHash = _passwordHasher.HashPassword(passwordResetDTO.Password);
            await _passwordResetTokenRepository.DeleteByEmail(user.Email);
            return await TryPerformUserUpdate(user);
        }

        // PATCH: api/v1/users/verified
        [HttpPatch("verified")]
        public async Task<ActionResult<IResponseObject>> PatchUserVerified(UserVerificationRequestDTO verificationDTO)
        {
            VerificationToken token = await _verificationTokenRepository.GetById(verificationDTO.VerificationTokenId);
            if (token == null || DateTime.Now >= token.Expiration)
                return BadRequest(UserUpdateErrorTitle, UserUpdateVerificationErrorDetail);
            User user = await _userRepository.GetByEmail(token.Email);
            if (user == null)
                return BadRequest(UserUpdateErrorTitle, UserUpdateVerificationUserErrorDetail);
            user.Verified = true;
            await _verificationTokenRepository.DeleteByEmail(user.Email);
            ActionResult<IResponseObject> response = await TryPerformUserUpdate(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return response;
        }

        // POST: api/v1/users
        [HttpPost]
        public async Task<ActionResult<IResponseObject>> PostUser(UserRequestDTO userDTO)
        {
            if (await _userRepository.GetByEmail(userDTO.Email) != null)
                return UserCreationConflict(EmailInUseErrorDetail);
            if (await _userRepository.GetByUsername(userDTO.Username) != null)
                return UserCreationConflict(UsernameTakenErrorDetail);
            User user = _mapper.Map<User>(userDTO);
            user.Email = user.Email;
            user.Username = user.Username;
            user.PasswordHash = _passwordHasher.HashPassword(userDTO.Password);
            user.Role = UserRole.User;
            user.Verified = false;
            await _userRepository.Create(user);
            string siteUrl = string.IsNullOrEmpty(userDTO.SiteUrlOverride) ?
                $"{HttpContext.Request.Scheme}{Uri.SchemeDelimiter}{HttpContext.Request.Host}" : userDTO.SiteUrlOverride;
            await _verificationTokenRepository.CreateDefaultAndEmail(user.Email, user.Username, siteUrl);
            return CreatedAtAction("GetUser", new { id = user.Id }, _mapper.Map<UserResponseDTO>(user));
        }

        // DELETE: api/v1/users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<IResponseObject>> DeleteUser(long id)
        {
            if (!IsSelfOrAdmin(id))
                return Forbidden();
            User user = await _userRepository.GetById(id);
            if (user == null)
                return UserNotFound();
            await _userRepository.Delete(id);
            return NoContent();
        }

        private bool IsSelfOrAdmin(long id)
        {
            long currentUserId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            string currentUserRole = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
            return currentUserId == id || currentUserRole == UserRole.Admin.ToString();
        }

        private async Task<ActionResult<IResponseObject>> TryPerformUserUpdate(User user)
        {
            try
            {
                await _userRepository.Update(user);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userRepository.ExistsInCurrentContext(user.Id))
                    return UserNotFound();
                else
                    throw;
            }
        }

        private ActionResult<IResponseObject> UserCreationConflict(string errorDetail)
        {
            ErrorResponseObject errorResponse = _responseObjectFactory
                .CreateErrorResponseObject(HttpStatusCode.Conflict, UserCreationErrorTitle, errorDetail);
            return Conflict(errorResponse);
        }

        private ActionResult<IResponseObject> Forbidden()
        {
            IResponseObject response = _responseObjectFactory
                .CreateErrorResponseObject(HttpStatusCode.Unauthorized, UpdateForbiddenTitle, UpdateForbiddenDetail);
            HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return new ObjectResult(response);
        }

        private ActionResult<IResponseObject> UserNotFound()
        {
            IResponseObject response = _responseObjectFactory
                .CreateErrorResponseObject(HttpStatusCode.NotFound, NotFoundTitle, NotFoundDetail);
            return NotFound(response);
        }

        private ActionResult<IResponseObject> BadRequest(string errorTitle, string errorDetail)
        {
            ErrorResponseObject errorResponse = _responseObjectFactory
                .CreateErrorResponseObject(HttpStatusCode.BadRequest, errorTitle, errorDetail);
            return BadRequest(errorResponse);
        }
    }
}
