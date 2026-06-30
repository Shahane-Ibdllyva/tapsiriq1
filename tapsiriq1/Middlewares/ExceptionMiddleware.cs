using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Application.Exceptions;
using FluentValidation; // <--- BU ƏLAVƏ EDİLDİ

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

        // --- BU METOD DƏYİŞDİ (async Task və ValidationException əlavə edildi) ---
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // 1. FLUENT VALIDATION XƏTALARI ÜÇÜN XÜSUSİ HANDELLEME (SİZİN GÖNDƏRDİYİNİZ KOD)
            if (exception is ValidationException validationEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                var errors = validationEx.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                await context.Response.WriteAsJsonAsync(new { Errors = errors });
                return; // Buradan çıx, digər kod işləməsin
            }

            // 2. QALAN CUSTOM EXCEPTIONLAR ÜÇÜN STANDART İDARƏETMƏ
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Server daxilində gözlənilməz xəta baş verdi.";

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
                    // Sistemsəl xətalar (NullReference və s.)
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = statusCode;

            // Köhnə formatda cavab göndərilir (sizin ErrorDetails class-ınızla)
            await context.Response.WriteAsync(new ErrorDetails
            {
                StatusCode = statusCode,
                Message = message
            }.ToString());
        }
    }
}