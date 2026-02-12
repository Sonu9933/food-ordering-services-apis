using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Customer.API.CustomMiddleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Pass the request to the next middleware
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error : {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Type = context.Response.ContentType,
                Title = "An error occurred while processing your request.",
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
