using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SinkingFunds.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SinkingFunds.Web.Middleware
{
    public class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        // Build and send the error response
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException ex) when (
                context.Request.Path.StartsWithSegments("/api") &&
                !context.Response.HasStarted)
            {
                var problem = new ValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        [string.Empty] = new[] { ex.Message }
                    })
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "One or more validation errors occurred."
                };

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                await context.Response.WriteAsJsonAsync(
                    problem,
                    options: null,
                    contentType: "application/problem+json",
                    cancellationToken: context.RequestAborted);
            }
        }
    }
}