var builder = WebApplication.CreateBuilder(args);

// Minimal pipeline for Phase 01 (no endpoints yet)
var app = builder.Build();

app.MapGet("/", () => Results.Ok("ThreadPilot.Insurances.Api"));

app.Run();
