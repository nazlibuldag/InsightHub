using FluentValidation;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace InsightHub.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(e => e.ErrorMessage).ToArray());

            var response = new
            {
                status = 400,
                message = "Doğrulama hatası oluştu.",
                errors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (KeyNotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 404,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 401,
                message = string.IsNullOrWhiteSpace(ex.Message) ? "Bu işlem için yetkiniz bulunmuyor." : ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (ArgumentException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 400,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (InvalidOperationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 400,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sunucuda beklenmeyen bir hata oluştu: {Message}", ex.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 500,
                message = ex.Message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}