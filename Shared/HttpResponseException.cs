using System;
using Microsoft.AspNetCore.Mvc;
using static Readible.Shared.HttpStatus;

namespace Readible.Shared
{
    public class HttpResponseException : Exception
    {
        public int StatusCode { get; }
        
        public HttpResponseException(int statusCode) : base(Message(statusCode))
        {
            StatusCode = statusCode;
        }

        public HttpResponseException(int statusCode, string content) : base(content)
        {
            StatusCode = statusCode;
        }

        public ActionResult ToResponse()
        {
            switch (StatusCode)
            {
                case BAD_REQUEST_CODE:
                    return new BadRequestObjectResult(Message);
                case FORBIDDEN_CODE:
                case UNAUTHORIZED_CODE:
                    return new UnauthorizedObjectResult(Message);
                case NOT_FOUND_CODE:
                    return new NotFoundObjectResult(Message);
                case CONFLICT_CODE:
                    return new ConflictObjectResult(Message);
                case UNPROCESSABLE_ENTITY:
                    return new UnprocessableEntityObjectResult(Message);
                default:
                    return new BadRequestObjectResult(Message);
            }
        }
    }
}