using Microsoft.AspNetCore.Http;
using RES.ATM.API.Shared.Exceptions;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace RES.ATM.API.ExceptionHandling.Exceptions
{
    public class ATMExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;

        public ATMExceptionHandlerMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    ATMValidationException => (int)HttpStatusCode.Unauthorized,
                    ATMNoFundsException => (int)HttpStatusCode.Forbidden,
                    ATMNoCashException => (int)HttpStatusCode.Forbidden,
                    ArgumentNullException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
