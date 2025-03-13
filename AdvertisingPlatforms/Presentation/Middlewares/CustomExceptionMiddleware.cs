using System.Net;
using AdvertisingPlatforms.Presentation.Dtos.Responses;

namespace AdvertisingPlatforms.Presentation.Middlewares;

public class CustomExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionMiddleware> _logger;
    
    public CustomExceptionMiddleware(RequestDelegate next, ILogger<CustomExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unexpected error occurred.");

        var response = exception switch
        {
            ApplicationException _ =>
                new ExceptionResponse(HttpStatusCode.BadRequest, "Application exception occurred."),
            FileNotFoundException _ =>
                new ExceptionResponse(HttpStatusCode.NotFound, "The file for reading platforms and locations was not found."),
            _ =>
                new ExceptionResponse(HttpStatusCode.InternalServerError, "Internal server error. Please retry later.")
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}