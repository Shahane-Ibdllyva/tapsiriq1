using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Application.Exceptions; // Custom xətalarımızı bura tanıtdıq
using tapsiriq1.Middlewares;

namespace tapsiriq1.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Standart olaraq 500 veririk
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Server daxilində gözlənilməz xəta baş verdi.";

            // MÜKƏMMƏL İDARƏETMƏ: Xətanın birbaşa tipinə baxırıq
            switch (exception)
            {
                case BadRequestException:
                    statusCode = (int)HttpStatusCode.BadRequest; // 400
                    message = exception.Message;
                    break;

                case UnauthorizedException: 
                    statusCode = (int)HttpStatusCode.Unauthorized; // 401
                    message = exception.Message;
                    break;

                case NotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound; // 404
                    message = exception.Message;
                    break;

                case ConflictException:
                    statusCode = (int)HttpStatusCode.Conflict; // 409
                    message = exception.Message;
                    break;

                default:
                    // Əgər bizim yazmadığımız sistemsəl bir xəta (məs: NullReference) olarsa bura düşür
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            }.ToString());
        }
    }
}