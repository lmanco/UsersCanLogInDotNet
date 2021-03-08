using UsersCanLogIn.API.Controllers.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UsersCanLogIn.API.Controllers.Filters
{
    public class InvalidModelStateFilter : ActionFilterAttribute
    {
        private const string ErrorTitle = "Invalid Attribute";

        private readonly IResponseObjectFactory _responseObjectFactory;

        public InvalidModelStateFilter(IResponseObjectFactory responseObjectFactory)
        {
            _responseObjectFactory = responseObjectFactory;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                IEnumerable<Error> errors = context.ModelState.Values.Where(entry => entry.ValidationState == ModelValidationState.Invalid)
                    .SelectMany(entry => entry.Errors
                    .Select(error => new Error { Status = HttpStatusCode.BadRequest, Title = ErrorTitle, Detail = error.ErrorMessage }));
                IResponseObject response = _responseObjectFactory.CreateErrorResponseObject(errors);
                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}
