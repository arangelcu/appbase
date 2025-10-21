using AppBase.API.Config.Middleware;

namespace AppBase.API.Config.Extensions;

public static class MetricsMiddlewareExtensions
{
    public static IApplicationBuilder UseSelectiveMetrics(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MetricsMiddleware>();
    }
}