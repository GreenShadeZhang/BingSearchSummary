var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.BingSearchSummary_ApiService>("apiservice");

builder.AddProject<Projects.BingSearchSummary_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

if (OperatingSystem.IsWindows())
{
    builder.AddProject<Projects.BingSearchSummary_WinUI>("client")
    .WithReference(apiService)
    .ExcludeFromManifest();
}
builder.Build().Run();
