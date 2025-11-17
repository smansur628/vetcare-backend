using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VetCare.Infrastructure.CrossCutting.Middleware;

public class CorrelationIdMiddleware : IMiddleware
{
    public const string Header = "X-Correlation-Id";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(Header, out var cid))
        {
            cid = Guid.NewGuid().ToString("N");
        }

        context.Items[Header] = cid.ToString();
        context.Response.Headers[Header] = cid.ToString();

        using (LogContext.PushProperty("CorrelationId", cid.ToString()))
        {
            await next(context);
        }
    }
}

