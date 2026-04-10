using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodOrderingServices.API.CustomMiddleware
{
    /// <summary>
    /// ASP.NET Core middleware that provides centralised, unhandled-exception handling for the entire request pipeline.
    /// </summary>
    /// <remarks>
    /// Registered as the outermost middleware in <c>Program.cs</c> so that any exception thrown by
    /// downstream middleware, controllers, or services is caught here before a response is written.
    /// All caught exceptions are:
    /// <list type="bullet">
    ///   <item><description>Logged at <c>Error</c> level via <see cref="ILogger{T}"/>.</description></item>
    ///   <item><description>Converted to a consistent <see cref="ProblemDetails"/> JSON response with HTTP 500.</description></item>
    /// </list>
    /// This prevents raw exception details from leaking to API consumers in production.
    /// </remarks>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initialises the middleware with the next delegate in the pipeline and a logger.
        /// </summary>
        /// <param name="next">The next middleware component to invoke if no exception occurs.</param>
        /// <param name="logger">Logger used to record exception details.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware, passing the request to the next component in the pipeline.
        /// Any unhandled exception is caught, logged, and transformed into a 500 response.
        /// </summary>
        /// <param name="httpContext">The HTTP context for the current request.</param>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                // Forward the request to the next middleware in the pipeline
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception Error : {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        /// <summary>
        /// Writes a standardised <see cref="ProblemDetails"/> error response with HTTP 500.
        /// </summary>
        /// <param name="context">The current HTTP context used to write the response.</param>
        /// <param name="ex">The exception that triggered this handler.</param>
        /// <returns>A <see cref="Task"/> that completes once the response has been written.</returns>
        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new ProblemDetails
            {
                Status = context.Response.StatusCode,
                Type   = context.Response.ContentType,
                Title  = "An error occurred while processing your request.",
            };

            return context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}
