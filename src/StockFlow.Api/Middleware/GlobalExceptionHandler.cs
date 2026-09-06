using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockFlow.Application.Inventory.Exceptions;

namespace StockFlow.Api.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is DbUpdateConcurrencyException)
        {
            Console.WriteLine("DbUpdateConcurrencyException 6666");
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Concurrency Conflict",
                Detail = "The inventory was modified by another request.",
                Extensions =
                    {
                        ["code"] = "CONCURRENCY_CONFLICT",
                        ["traceId"] = httpContext.TraceIdentifier,
                    }
            };

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
        else if (exception is InsufficientInventoryException)
        {
            Console.WriteLine("Insufficient inventory exception occurred.");
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Insufficient Inventory",
                Detail = "The inventory is insufficient for the requested operation.",
                Extensions =
                    {
                        ["code"] = "INSUFFICIENT_INVENTORY",
                        ["traceId"] = httpContext.TraceIdentifier,
                    }
            };

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }
        else
        {
            Console.WriteLine("Unhandled exception occurred.");
            // _logger.LogError(exception, "Unhandled exception occurred.");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred.",
            };

            problemDetails.Extensions["code"] = "INTERNAL_SERVER_ERROR";
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}