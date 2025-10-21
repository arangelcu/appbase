using Microsoft.AspNetCore.Mvc.Filters;

namespace AppBase.Config.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MetricsAttribute : ActionFilterAttribute
{
}