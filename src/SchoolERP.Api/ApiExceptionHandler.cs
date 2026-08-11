using System.ComponentModel.DataAnnotations;
using System.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SchoolERP.Api;

internal sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var error = Map(exception);

        if (error.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled request failure");
        }
        else
        {
            logger.LogWarning(exception, "Request failed with {ErrorCode}", error.Code);
        }

        httpContext.Response.StatusCode = error.Status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = error.Status,
                Title = error.Title,
                Extensions = { ["code"] = error.Code }
            }
        });
    }

    private static ApiError Map(Exception exception) => exception switch
    {
        ValidationException => new(StatusCodes.Status400BadRequest, "Validation", "Validation failed."),
        UnauthorizedAccessException => new(StatusCodes.Status401Unauthorized, "Unauthorized", "Authentication is required."),
        SecurityException => new(StatusCodes.Status403Forbidden, "Forbidden", "Access is forbidden."),
        KeyNotFoundException => new(StatusCodes.Status404NotFound, "NotFound", "The requested resource was not found."),
        NotSupportedException => new(StatusCodes.Status422UnprocessableEntity, "BusinessRuleViolation", "The requested operation is not supported."),
        InvalidOperationException => new(StatusCodes.Status409Conflict, "Conflict", "The request conflicts with the current state."),
        _ => new(StatusCodes.Status500InternalServerError, "Unexpected", "An unexpected error occurred.")
    };

    private sealed record ApiError(int Status, string Code, string Title);
}
