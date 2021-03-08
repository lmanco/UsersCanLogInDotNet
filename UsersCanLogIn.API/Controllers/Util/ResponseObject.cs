using System.Collections.Generic;
using System.Net;

namespace UsersCanLogIn.API.Controllers.Util
{
    public interface IResponseObject
    {
    }

    public class MultiItemResponseObject : IResponseObject
    {
        public IEnumerable<object> Data { get; set; }
    }

    public class SingleItemResponseObject : IResponseObject
    {
        public object Data { get; set; }
    }

    public class ErrorResponseObject : IResponseObject
    {
        public IEnumerable<Error> Errors { get; set; }
    }

    public class Error
    {
        public HttpStatusCode Status { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
    }
}
