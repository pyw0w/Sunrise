using System.Net;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using Sunrise.Server.Helpers;
using Sunrise.Server.Utils;

namespace Sunrise.Server;

public sealed class Middleware(
    IMemoryCache cache
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var ip = RegionHelper.GetUserIpAddress(context.Request);

        if (Configuration.BannedIps.Contains(ip.ToString()))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        if (context.Request.Path.StartsWithSegments("/metrics") && !Equals(ip, IPAddress.Loopback))
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        var limiter = GetRateLimiter(ip);
        using var lease = await limiter.AcquireAsync(1, context.RequestAborted);

        if (limiter.GetStatistics() is { } statistics)
        {
            context.Response.Headers["X-RateLimit-Limit"] = $"{Configuration.ServerRateLimit}";
            context.Response.Headers["X-RateLimit-Remaining"] = $"{statistics.CurrentAvailablePermits}";
            if (lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                context.Response.Headers.RetryAfter = $"{retryAfter.Seconds}";
        }

        if (lease.IsAcquired is false)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        await next(context);
    }

    private RateLimiter GetRateLimiter(IPAddress key)
    {
        return cache.GetOrCreate(key,
            entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromSeconds(Configuration.ServerRateLimitWindow);
                return new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                {
                    AutoReplenishment = true,
                    TokenLimit = Configuration.ServerRateLimit,
                    TokensPerPeriod = Configuration.ServerRateLimit,
                    QueueLimit = 0,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(Configuration.ServerRateLimitWindow)
                });
            }) ?? throw new InvalidOperationException($"Failed to create rate limiter for {key}");
    }
}