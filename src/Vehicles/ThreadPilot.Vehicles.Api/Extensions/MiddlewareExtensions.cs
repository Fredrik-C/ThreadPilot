using ThreadPilot.Vehicles.Api.Middleware;

namespace ThreadPilot.Vehicles.Api.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}