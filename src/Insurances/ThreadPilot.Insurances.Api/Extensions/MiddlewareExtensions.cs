using ThreadPilot.Insurances.Api.Middleware;

namespace ThreadPilot.Insurances.Api.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}