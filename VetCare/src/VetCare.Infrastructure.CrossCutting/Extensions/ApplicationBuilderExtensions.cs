using Microsoft.AspNetCore.Builder;
using VetCare.Infrastructure.CrossCutting.Middleware;

namespace VetCare.Infrastructure.CrossCutting.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCrossCutting(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}

