var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.AspireHealthCheckBug_ApiService>("apiservice")
    .WithHttpHealthCheck("/health", 200, "http");

builder.AddProject<Projects.AspireHealthCheckBug_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithHttpHealthCheck("/health", 200, "http");

builder.Build().Run();
