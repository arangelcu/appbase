using AppBase.Config.Middleware;

namespace AppBase.Config.Extensions;

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseSelectiveMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}