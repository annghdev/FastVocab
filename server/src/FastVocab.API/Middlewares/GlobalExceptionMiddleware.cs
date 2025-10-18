using FastVocab.Shared.Utils;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace FastVocab.API.Middlewares;

/// <summary>
/// Global exception handling middleware
/// Catches unhandled exceptions and returns appropriate HTTP responses
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode = statusCode,
                    error = "Validation failed",
                    errors = validationException.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        message = e.ErrorMessage,
                        code = 400
                    })
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode = statusCode,
                    error = "An internal server error occurred",
                    message = exception.Message
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        // Log the exception
        if (statusCode == (int)HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Validation exception occurred: {Exception}", exception.Message);
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

