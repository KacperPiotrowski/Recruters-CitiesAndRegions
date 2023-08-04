using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CitiesAndRegions.Infrastructure.Middlewares;

public sealed class SetResponseTimeMiddleware
{
    private readonly ILogger<SetResponseTimeMiddleware> _logger;
    private readonly RequestDelegate _next;

    public SetResponseTimeMiddleware(RequestDelegate next,
                                     ILogger<SetResponseTimeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var watch = new Stopwatch();
        watch.Start();

        httpContext.Response.OnStarting(() =>
        {
            watch.Stop();
            httpContext.Response.Headers.Add("x-time", $"{watch.ElapsedMilliseconds}ms");

            _logger.LogInformation("For {path} time is {time}", httpContext.Request.Path, watch.ElapsedMilliseconds);

            return Task.CompletedTask;
        });

        await _next(httpContext);
        watch.Stop();
    }
}

