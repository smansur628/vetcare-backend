using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCare.Infrastructure.CrossCutting.Interfaces;

namespace VetCare.Infrastructure.CrossCutting.Middleware;

public class TenantMiddleware : IMiddleware
{
    private readonly IHashIdService _hash;

    public TenantMiddleware(IHashIdService hash) => _hash = hash;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 1) Header
        if (context.Request.Headers.TryGetValue("X-Clinic", out var code)) {
            context.Items["TenantId"] = _hash.Decode(code!);
        }            

        // 2) JWT Claim ‘cli’
        else if (context.User.Identity?.IsAuthenticated == true)
        {
            var claim = context.User.FindFirst("cli")?.Value;
            if (claim is not null)
            {
                context.Items["TenantId"] = _hash.Decode(claim);
            }                
        }

        if (!context.Items.ContainsKey("TenantId"))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("Clinic id missing");
            return;
        }

        await next(context);
    }
}

