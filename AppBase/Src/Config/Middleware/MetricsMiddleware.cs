using System.Diagnostics;
using AppBase.Config.Filters;
using Prometheus;

namespace AppBase.Config.Middleware;

public class MetricsMiddleware
{
    private static readonly Counter _requestCounter = Metrics
        .CreateCounter("custom_controller_requests_total",
            "Total requests to annotated controllers",
            new CounterConfiguration
            {
                LabelNames = new[] { "controller", "action", "method", "status", "path" }
            });

    private static readonly Histogram _requestDuration = Metrics
        .CreateHistogram("custom_controller_duration_seconds",
            "Request duration for monitored requests",
            new HistogramConfiguration
            {
                LabelNames = new[] { "controller", "action", "path" },
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 16)
            });

    private readonly RequestDelegate _next;

    public MetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var hasMetricsAttribute = endpoint?.Metadata.GetMetadata<MetricsAttribute>() != null;

        if (!hasMetricsAttribute)
        {
            await _next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var method = context.Request.Method;
        var path = context.Request.Path;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var controller = context.GetRouteValue("controller")?.ToString() ?? "Unknown";
            var action = context.GetRouteValue("action")?.ToString() ?? "Unknown";
            var statusCode = context.Response.StatusCode;

            _requestCounter
                .WithLabels(controller, action, method, statusCode.ToString(), path)
                .Inc();

            _requestDuration
                .WithLabels(controller, action, path)
                .Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}