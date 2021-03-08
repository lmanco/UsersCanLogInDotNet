using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.Controllers.Util
{
    public interface IResponseObjectFactory
    {
        MultiItemResponseObject CreateResponseObject(IEnumerable<object> enumerable);
        SingleItemResponseObject CreateResponseObject(object obj);
        ErrorResponseObject CreateErrorResponseObject(HttpStatusCode statusCode, string title, string detail);
        ErrorResponseObject CreateErrorResponseObject(IEnumerable<Error> errors);
    }

    public class ResponseObjectFactory : IResponseObjectFactory
    {
        public MultiItemResponseObject CreateResponseObject(IEnumerable<object> enumerable)
        {
            return new MultiItemResponseObject { Data = enumerable };
        }

        public SingleItemResponseObject CreateResponseObject(object obj)
        {
            return new SingleItemResponseObject { Data = obj };
        }

        public ErrorResponseObject CreateErrorResponseObject(HttpStatusCode statusCode, string title, string detail)
        {
            return new ErrorResponseObject {
                Errors = new Error[] {
                    new Error { Status = statusCode, Title = title, Detail = detail }
                }
            };
        }

        public ErrorResponseObject CreateErrorResponseObject(IEnumerable<Error> errors)
        {
            return new ErrorResponseObject { Errors = errors };
        }
    }
}
