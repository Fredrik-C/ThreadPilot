using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Compact;
using ThreadPilot.Insurances.Api.Extensions;
using ThreadPilot.Insurances.Application.Services;
using ThreadPilot.Insurances.Domain;
using ThreadPilot.Insurances.Infrastructure.Providers;
using ThreadPilot.Insurances.Infrastructure.Clients;
using Microsoft.AspNetCore.Mvc;
using ThreadPilot.Insurances.Api.ModelBinding;

var builder = WebApplication.CreateBuilder(args);

// Bind options
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection(SecurityOptions.sectionName));
builder.Services.Configure<StubInsuranceOptions>(builder.Configuration.GetSection(StubInsuranceOptions.sectionName));
builder.Services.Configure<VehiclesApiClientOptions>(builder.Configuration.GetSection(VehiclesApiClientOptions.SectionName));
var securityOptions = builder.Configuration.GetSection(SecurityOptions.sectionName).Get<SecurityOptions>() ?? new SecurityOptions();

// Serilog configuration: JSON console, enrich with correlation
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Service", "InsurancesApi")
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Register custom model binder provider for SwedishPersonalId
    options.ModelBinderProviders.Insert(0, new SwedishPersonalIdModelBinderProvider());
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new ProblemDetails
        {
            Title = "Invalid Personal ID",
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
        };
        return new ObjectResult(problemDetails) { StatusCode = problemDetails.Status };
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks (basic for now; DB and external deps can be added in later phases)
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

// Insurance services
builder.Services.AddScoped<IInsuranceProvider, StubInsuranceProvider>();
builder.Services.AddScoped<InsuranceService>();
builder.Services.AddHttpClient<VehiclesApiClient, VehiclesApiClient>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ThreadPilot.Insurances.Application.Clients.IVehiclesApiClient>(sp => sp.GetRequiredService<VehiclesApiClient>());

// AuthN/AuthZ with toggle
if (securityOptions.Enabled)
{
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            // Configurable authority/audience via appsettings; placeholders for now
            options.Authority = builder.Configuration[$"{SecurityOptions.sectionName}:Authority"]; // e.g., https://login.example.com/
            options.Audience = builder.Configuration[$"{SecurityOptions.sectionName}:Audience"];   // e.g., api://insurances
            options.RequireHttpsMetadata = false;
        });
    builder.Services.AddAuthorization(options =>
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
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(_ => { });
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("ReadAccess", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("WriteAccess", policy => policy.RequireAssertion(_ => true));
        options.AddPolicy("AllowAnonymousWhenAuthDisabled", policy => policy.RequireAssertion(_ => true));
    });
}

// OpenTelemetry: traces and metrics
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService(serviceName: "ThreadPilot.Insurances.Api"))
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation();
        t.AddHttpClientInstrumentation();
        // exporter via config; default console for dev
        t.AddConsoleExporter();
    })
    .WithMetrics(m =>
    {
        m.AddAspNetCoreInstrumentation();
        m.AddRuntimeInstrumentation();
        m.AddHttpClientInstrumentation();
        m.AddConsoleExporter();
    });

var app = builder.Build();

// Global exception handling middleware
app.UseGlobalExceptionHandling();

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => true });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });

if (securityOptions.Enabled)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();

app.Run();

internal sealed record SecurityOptions
{
    public const string sectionName = "Security";
    public bool Enabled { get; init; } = true;
    public string? Authority { get; init; }
    public string? Audience { get; init; }
}
