using CrochetAI.Api.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CrochetAI.Api.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IRedisService _redisService;

    public RedisHealthCheck(IRedisService redisService)
    {
        _redisService = redisService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var isConnected = await _redisService.IsConnectedAsync();
            if (isConnected)
            {
                return HealthCheckResult.Healthy("Redis is connected");
            }
            return HealthCheckResult.Unhealthy("Redis is not connected");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed", ex);
        }
    }
}
