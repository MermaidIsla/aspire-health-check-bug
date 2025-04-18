# Aspire Health Check Bug

This repository showcases a bug in .NET Aspire.

## Steps to reproduce this bug

1. Create a new project using the template `.NET Aspire Starter App`
2. In the `Web` project create a new class called `ApiServiceHealthCheck`
3. Copy the following code into `ApiServiceHealthCheck.cs` file:
```csharp
public class ApiServiceHealthCheck(HttpClient httpClient) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync("http://apiservice/health", cancellationToken).ConfigureAwait(false);

        return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
}
```
4. In the `Web` project open `Program.cs` file and add the following code `builder.Services.AddHealthChecks().AddCheck<ApiServiceHealthCheck>("apiservice");` just before `var app = builder.Build();`
5. In the `AppHost` project ensure that both projects (`apiservice` and `webfrontend`) have `.WithHttpHealthCheck("/health", 200, "http")`
6. Run the `AppHost` project, open the dashboard and click on `Traces` tab
7. You should now see a health check spam

## A possible way to fix this bug

1. In the `ServiceDefaults` project open `Extensions.cs` file and edit `AddHttpClientInstrumentation()` method found inside `.WithTracing(tracing => ... omitted for abbreviation ...)` in `ConfigureOpenTelemetry` method:
```csharp
.AddHttpClientInstrumentation(options =>
{
    options.FilterHttpRequestMessage = (request) =>
    {
        return !request.RequestUri?.PathAndQuery.Contains("/health") ?? true;
    };
})
```
2. Run the `AppHost` project, open the dashboard and click on `Traces` tab
3. You should no longer see any health check spam

**To consider:** In theory there could be a request that gets filtered out but has nothing to do with a health check.