using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ThreadPilot.Insurances.Domain.Exceptions;

namespace ThreadPilot.Insurances.Api.Middleware;

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
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Argument exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Unauthorized access exception occurred");
            await HandleExceptionAsync(context, ex).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "HTTP request exception occurred");
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
            ValidationException => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            ArgumentException => new ProblemDetails
            {
                Title = "Bad Request",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.Unauthorized,
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            },
            NotFoundException => new ProblemDetails
            {
                Title = "Not Found",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            KeyNotFoundException => new ProblemDetails
            {
                Title = "Not Found",
                Detail = exception.Message,
                Status = (int)HttpStatusCode.NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            HttpRequestException httpEx => new ProblemDetails
            {
                Title = "Service Unavailable",
                Detail = httpEx.Message,
                Status = (int)(httpEx.StatusCode ?? HttpStatusCode.ServiceUnavailable),
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

        // Add stable machine-readable error code
        problemDetails.Extensions["code"] = exception switch
        {
            ValidationException => "ValidationError",
            NotFoundException => "NotFound",
            ArgumentException => "BadRequest",
            UnauthorizedAccessException => "Unauthorized",
            TimeoutException => "Timeout",
            OperationCanceledException => "Cancelled",
            HttpRequestException => "UpstreamHttpError",
            InvalidOperationException => "InvalidOperation",
            _ => "InternalServerError"
        };

        // Add observability context
        problemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
        if (context.Request.Headers.TryGetValue("X-Correlation-ID", out var cid))
        {
            problemDetails.Extensions["correlationId"] = cid.ToString();
        }

        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var json = System.Text.Json.JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json).ConfigureAwait(false);
    }
}