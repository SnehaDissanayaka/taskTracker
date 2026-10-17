using System.Globalization;
using System.Net.Sockets;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TaskTracker.Api.RateLimiting;

public class RateLimitSettings
{
    public int PerIpPerMinute { get; set; } = 120;
    public int PerIpWritesPerMinute { get; set; } = 30;
    public int GlobalPerMinute { get; set; } = 600;
    public int GlobalCreatesPerHour { get; set; } = 600;
}

public static class RateLimitingExtensions
{
    // Limits are per client IP (never per X-Client-Id, which a caller can rotate freely), plus global ceilings
    // that protect the free-tier CPU and database quotas and bound how fast rows can accumulate.
    public static IServiceCollection AddAbuseProtection(this IServiceCollection services, RateLimitSettings settings)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
                }

                return ValueTask.CompletedTask;
            };

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                FixedWindow(ctx => $"ip:{ClientKey(ctx)}", settings.PerIpPerMinute, TimeSpan.FromMinutes(1)),
                FixedWindow(ctx => IsWrite(ctx) ? $"ip-writes:{ClientKey(ctx)}" : null, settings.PerIpWritesPerMinute, TimeSpan.FromMinutes(1)),
                FixedWindow(_ => "global", settings.GlobalPerMinute, TimeSpan.FromMinutes(1)),
                FixedWindow(ctx => HttpMethods.IsPost(ctx.Request.Method) ? "global-creates" : null, settings.GlobalCreatesPerHour, TimeSpan.FromHours(1)));
        });

        return services;
    }

    private static PartitionedRateLimiter<HttpContext> FixedWindow(Func<HttpContext, string?> partitionKey, int permitLimit, TimeSpan window) =>
        PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            partitionKey(ctx) is { } key
                ? RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = window,
                })
                : RateLimitPartition.GetNoLimiter("unlimited"));

    private static bool IsWrite(HttpContext ctx) =>
        HttpMethods.IsPost(ctx.Request.Method) || HttpMethods.IsPut(ctx.Request.Method) ||
        HttpMethods.IsDelete(ctx.Request.Method) || HttpMethods.IsPatch(ctx.Request.Method);

    // IPv4 clients are keyed by address; IPv6 clients by their /64 prefix, because one host usually controls its whole /64.
    private static string ClientKey(HttpContext ctx)
    {
        var address = ctx.Connection.RemoteIpAddress;
        if (address is null) return "unknown";
        if (address.IsIPv4MappedToIPv6) address = address.MapToIPv4();
        if (address.AddressFamily != AddressFamily.InterNetworkV6) return address.ToString();
        return Convert.ToHexString(address.GetAddressBytes(), 0, 8) + "/64";
    }
}
