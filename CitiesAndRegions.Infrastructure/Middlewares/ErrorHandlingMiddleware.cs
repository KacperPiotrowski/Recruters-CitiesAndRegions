using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using CitiesAndRegions.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CitiesAndRegions.Infrastructure.Middlewares;

public sealed class ErrorHandlingMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Debug.Assert(context is not null);
        Debug.Assert(exception is not null);
        string result = null;

        switch (exception)
        {
            case ApiException ae:
                context.Response.StatusCode = (int)ae.Code;
                result = JsonSerializer.Serialize(ae.Error);
                _logger.LogError(ae, ae.Code.ToString());
                break;
            case { } e:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                result = JsonSerializer.Serialize(e);
                _logger.LogError(e, "Unhandled Exception");
                break;
        }
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(result);
    }
}

