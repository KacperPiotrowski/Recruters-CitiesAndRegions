using Microsoft.AspNetCore.Builder;

namespace CitiesAndRegions.Infrastructure.Middlewares;

public static class WebApplicationExtensions
{
    public static IApplicationBuilder AddMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<SetResponseTimeMiddleware>();

        return app;
    }
}
