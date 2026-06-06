using System.Net;
using API.Models;
using Domain.Exceptions;

namespace API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
            _logger.LogError(ex, "Error no controlado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var environment = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        var includeDetails = environment.IsDevelopment();

        switch (exception)
        {
            case Domain.Exceptions.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail(
                        "La validación de la solicitud falló.",
                        validationEx.Errors.SelectMany(e => e.Value)));
                break;
            case NotFoundException:
            case KeyNotFoundException:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("Recurso no encontrado."));
                break;
            case UnauthorizedAccessException:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail("No autorizado."));
                break;
            case DomainException:
            case ArgumentException:
            case InvalidOperationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(exception.Message));
                break;
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(
                    ApiResponse<object>.Fail(
                        "Ha ocurrido un error interno del servidor.",
                        includeDetails && !string.IsNullOrWhiteSpace(exception.StackTrace)
                            ? new[] { exception.Message, exception.StackTrace }
                            : new[] { "Ha ocurrido un error interno del servidor." }));
                break;
        }
    }
}

// Extension method para facilitar el registro
public static class GlobalExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
