using UsersCanLogIn.API.Controllers.Util;
using UsersCanLogIn.API.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace UsersCanLogIn.API.Controllers.Filters
{
    public class RestrictByRolesAttribute : ActionFilterAttribute
    {
        private const string ErrorTitle = "Forbidden";
        private const string ErrorMessage = "Authorization is required to perform this action.";

        public UserRole[] _userRoles;

        public RestrictByRolesAttribute(params UserRole[] userRoles)
        {
            _userRoles = userRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var responseObjectFactory = (IResponseObjectFactory)context.HttpContext.RequestServices.GetService(typeof(IResponseObjectFactory));
            string userRole = context.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            if (!_userRoles.Select(role => role.ToString()).Contains(userRole))
            {
                IResponseObject response = responseObjectFactory.CreateErrorResponseObject(HttpStatusCode.Forbidden, ErrorTitle, ErrorMessage);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Result = new ObjectResult(response);
            }
        }
    }
}
