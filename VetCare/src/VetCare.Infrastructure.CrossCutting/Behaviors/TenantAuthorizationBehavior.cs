using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetCare.Infrastructure.CrossCutting.Interfaces;

namespace VetCare.Infrastructure.CrossCutting.Behaviors;

public sealed class TenantAuthorizationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ITenantScoped
{
    private readonly IHttpContextAccessor _http;
    public TenantAuthorizationBehavior(IHttpContextAccessor http) => _http = http;

    public Task<TResponse> Handle(TRequest request,
                                  RequestHandlerDelegate<TResponse> next,
                                  CancellationToken cancellationToken)
    {
        var tenant = (int?)_http.HttpContext?.Items["TenantId"];
        if (tenant == null || tenant != request.ClinicaId)
        {
            throw new UnauthorizedAccessException("Tenant mismatch");
        }           

        return next();
    }
}

