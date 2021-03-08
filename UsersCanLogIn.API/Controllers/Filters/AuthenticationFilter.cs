using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.Controllers.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        private const string ErrorTitle = "Unauthorized";
        private const string ErrorMessage = "Authentication is required to perform this action.";
        private static readonly string[] ActionWhitelist = { "PostLogin", "PostUser", "PatchUserPassword",
            "PatchUserVerified", "PostVerificationToken", "PostPasswordResetToken", "GetPasswordResetToken" };

        private readonly IUserRepository _userRepository;
        private readonly IResponseObjectFactory _responseObjectFactory;

        public AuthenticationFilter(IUserRepository userRepository, IResponseObjectFactory responseObjectFactory)
        {
            _userRepository = userRepository;
            _responseObjectFactory = responseObjectFactory;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (ActionWhitelist.Contains(context.ActionDescriptor.RouteValues["action"]))
                await next();
            else
            {
                string userIdString = context.HttpContext.User.Claims
                    .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!long.TryParse(userIdString, out long userId) || await _userRepository.GetById(userId) == null)
                {
                    var errors = new Error[] { new Error { Status = HttpStatusCode.Unauthorized, Title = ErrorTitle, Detail = ErrorMessage } };
                    IResponseObject response = _responseObjectFactory.CreateErrorResponseObject(errors);
                    context.Result = new UnauthorizedObjectResult(response);
                }
                else
                    await next();
            }
        }
    }
}
