using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspireHealthCheckBug.Web
{
    public class ApiServiceHealthCheck(HttpClient httpClient) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync("http://apiservice/health", cancellationToken).ConfigureAwait(false);

            return response.IsSuccessStatusCode ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
