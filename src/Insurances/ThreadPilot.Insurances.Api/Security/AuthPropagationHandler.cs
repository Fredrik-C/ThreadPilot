using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;

namespace ThreadPilot.Insurances.Api.Security;

[SuppressMessage("Performance", "CA1812", Justification = "Instantiated by DI container")]
internal sealed class AuthPropagationHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is not null && !request.Headers.Contains("Authorization"))
        {
            if (ctx.Request.Headers.TryGetValue("Authorization", out var authValues))
            {
                var raw = authValues.ToString();
                if (!string.IsNullOrWhiteSpace(raw) && AuthenticationHeaderValue.TryParse(raw, out var parsed))
                {
                    request.Headers.Authorization = parsed;
                }
            }
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}

