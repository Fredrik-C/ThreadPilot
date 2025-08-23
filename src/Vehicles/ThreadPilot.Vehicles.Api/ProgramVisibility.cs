using System.Diagnostics.CodeAnalysis;

// Expose Program type for WebApplicationFactory
[SuppressMessage("Design", "CA1515", Justification = "Public type required for WebApplicationFactory in tests")]
[SuppressMessage("Performance", "CA1812", Justification = "Activated by test host via reflection")]
public sealed partial class Program { }

