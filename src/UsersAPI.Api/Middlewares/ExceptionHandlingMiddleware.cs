using System.Net;
using System.Text.Json;
using UsersAPI.Application.Common.Exceptions;

namespace UsersAPI.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnauthorizedException ex)
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.Unauthorized,
                ex.Message);
        }
        catch (Exception)
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode status,
        string message)
    {
        context.Response.StatusCode = (int)status;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            error = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(payload));
    }
}
