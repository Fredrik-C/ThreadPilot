using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ThreadPilot.Vehicles.Api.Middleware;

// Suppress CA1812 - This middleware is instantiated by the ASP.NET Core framework
#pragma warning disable CA1812
internal sealed class GlobalExceptionHandlingMiddleware
#pragma warning restore CA1812
{
    private readonly RequestDelegate next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(logger);

        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        try
        {
            await next(context).ConfigureAwait(false);
        }
        // Suppress CA1848 - LoggerMessage delegates are a performance optimization
#pragma warning disable CA1848 // Use the LoggerMessage delegates
        catch (TimeoutException ex)
        {
            logger.LogError(ex, "Timeout exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Operation canceled exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Invalid operation exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        // Suppress CA1031 - We intentionally catch all exceptions to handle them gracefully
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
#pragma warning restore CA1031 // Do not catch general exception types
#pragma warning restore CA1848 // Use the LoggerMessage delegates
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = exception switch
        {
            TimeoutException => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = "External service timeout",
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            },
            OperationCanceledException => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = "Operation was cancelled",
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
            },
            InvalidOperationException => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "External service error",
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            },
            _ => new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred",
                Status = (int)HttpStatusCode.InternalServerError,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var json = System.Text.Json.JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json).ConfigureAwait(false);
    }
}