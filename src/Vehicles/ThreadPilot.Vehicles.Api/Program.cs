using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Compact;

var builder = WebApplication.CreateBuilder(args);

// Bind options
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.sectionName));
var securityOptions = builder.Configuration.GetSection(SecurityOptions.sectionName).Get<SecurityOptions>() ?? new SecurityOptions();

// Serilog configuration: JSON console, enrich with correlation
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "VehiclesApi")
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

// OpenAPI
_ = builder.Services.AddEndpointsApiExplorer();
_ = builder.Services.AddSwaggerGen();

// Health checks (basic for now; DB and external deps can be added in later phases)
_ = builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());


// AuthN/AuthZ with toggle
if (securityOptions.Enabled)
{
    _ = builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Configurable authority/audience via appsettings; placeholders for now
            options.Authority = builder.Configuration[$"{SecurityOptions.sectionName}:Authority"]; // e.g., https://login.example.com/
            options.Audience = builder.Configuration[$"{SecurityOptions.sectionName}:Audience"];   // e.g., api://vehicles
            options.RequireHttpsMetadata = false;
        });
    _ = builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ReadAccess", policy =>
            policy.RequireAuthenticatedUser().RequireRole("Reader"));
        options.AddPolicy("WriteAccess", policy =>
            policy.RequireAuthenticatedUser().RequireRole("Writer"));
    });
}
else
{
    // Add empty auth services so [Authorize] can still resolve when disabled
    _ = builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(_ => { });
    _ = builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ReadAccess", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("WriteAccess", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("AllowAnonymousWhenAuthDisabled", policy => policy.RequireAssertion(_ => true));
    });
}

// OpenTelemetry: traces and metrics
_ = builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(serviceName: "ThreadPilot.Vehicles.Api"))
    .WithTracing(t =>
    {
        _ = t.AddAspNetCoreInstrumentation();
        _ = t.AddHttpClientInstrumentation();
        // exporter via config; default console for dev
        _ = t.AddConsoleExporter();
    })
    .WithMetrics(m =>
    {
        _ = m.AddAspNetCoreInstrumentation();
        _ = m.AddRuntimeInstrumentation();
        _ = m.AddHttpClientInstrumentation();
        _ = m.AddConsoleExporter();
    });

var app = builder.Build();

// Correlation ID middleware (prefer X-Correlation-ID header, otherwise use TraceIdentifier)
app.Use(async (context, next) =>
{
    var hasHeader = context.Request.Headers.TryGetValue("X-Correlation-ID", out var headerVal);
    var correlationId = hasHeader ? headerVal.ToString() : context.TraceIdentifier;

    context.Response.Headers["X-Correlation-ID"] = correlationId;

    using (LogContext.PushProperty("CorrelationId", correlationId))
    using (LogContext.PushProperty("RequestPath", context.Request.Path))
    {
        await next().ConfigureAwait(false);
    }
});

// Swagger in Development
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

_ = app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => true });
_ = app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });

if (securityOptions.Enabled)
{
    _ = app.UseAuthentication();
    _ = app.UseAuthorization();
}

_ = app.MapGet("/", () => Results.Ok("ThreadPilot.Vehicles.Api"))
    .RequireAuthorization("ReadAccess");

app.Run();

internal sealed record SecurityOptions
{
    public const string sectionName = "Security";
    public bool Enabled { get; init; } = true;
    public string? Authority { get; init; }
    public string? Audience { get; init; }
}
