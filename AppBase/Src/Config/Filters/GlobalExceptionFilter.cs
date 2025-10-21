using AppBase.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AppBase.Config.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        if (IsConcurrencyException(context.Exception))
        {
            _logger.LogWarning(context.Exception.Message, "🔀 Global concurrency filter");

            context.Result = new ConflictObjectResult(new
            {
                Message = Message.Error_Concurrency,
                ErrorType = "Concurrency"
            });
            context.ExceptionHandled = true;
        }

        if (IsUniqueConstraintViolation(context.Exception))
        {
            _logger.LogError(context.Exception.Message, "🔀 Global concurrency filter");

            context.Result = new ConflictObjectResult(new
            {
                Message = Message.Error_Constraint,
                ErrorType = "ConstraintViolation"
            });
            context.ExceptionHandled = true;
        }
        else
        {
            _logger.LogError(context.Exception.Message, "❌ Global exception filter");

            context.Result = new ObjectResult(new
            {
                Message = Message.Error_General,
                Error = context.Exception.Message
            })
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
        }
    }

    /**
     * Verify if Exception is a ConcurrencyException
     */
    private static bool IsConcurrencyException(Exception ex)
    {
        var current = ex;
        while (current != null)
        {
            if (current is PostgresException pgEx && pgEx.SqlState == "40001") return true;
            if (current is DbUpdateConcurrencyException) return true;
            if (current.Message?.Contains("could not serialize access due to concurrent update") == true) return true;
            if (current.Message?.Contains("4001") == true) return true;
            current = current.InnerException;
        }

        return false;
    }

    /**
     * Verify if Exception is a UniqueConstraintViolation
     */
    private static bool IsUniqueConstraintViolation(Exception ex)
    {
        return ex.InnerException is PostgresException pgEx &&
               pgEx.SqlState == "23505";
    }
}