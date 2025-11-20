using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
//using Dapper;
using MediatR;                                       // <- interfaces
//using FluentValidation;
//using VetCare.Infrastructure.Persistence;
//using VetCare.Domain.Repositories;
//using VetCare.Infrastructure.Repositories;
using VetCare.Infrastructure.CrossCutting.Services;
using VetCare.Infrastructure.CrossCutting.Interfaces;
using VetCare.Infrastructure.CrossCutting.Behaviors;
using VetCare.Infrastructure.CrossCutting.Options;
using VetCare.Infrastructure.CrossCutting.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VetCare.Infrastructure.Data.Context;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrossCutting(
        this IServiceCollection services, IConfiguration cfg)
    {
        // Options
        services.Configure<JwtOptions>(cfg.GetSection("Security:Jwt"));
        services.Configure<HashIdOptions>(cfg.GetSection("Security:HashId"));

        // Services
        services.AddSingleton<IJwtService, JwtService>();
        services.AddSingleton<IHashIdService, HashIdService>();

        // MediatR Behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantAuthorizationBehavior<,>));

        // Middlewares
        services.AddTransient<ExceptionHandlingMiddleware>();
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<TenantMiddleware>();

        // JWT Authentication
        var jwt = cfg.GetSection("Security:Jwt").Get<JwtOptions>()!;
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(op =>
                {
                    op.TokenValidationParameters = new()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwt.Secret))
                    };
                });

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        return services;
    }

    public static IServiceCollection AddVetCareCore(
    this IServiceCollection services, IConfiguration cfg)
    {
        // 1. Banco de Dados ----------------------------------------
        var cs = cfg.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<VetCareContext>(opt =>
        opt.UseNpgsql(cs)
        .UseSnakeCaseNamingConvention());            // EF Core

        //services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(cs)); // Dapper

        // 2. Repositórios -----------------------------------------
        //        services.AddScoped<IClinicaRepository, ClinicaRepository>();
        // services.AddScoped<IProdutoRepository, ProdutoRepository>();
        // ...

        // 3. MediatR + FluentValidation ---------------------------
        var assemblyDomain = AppDomain.CurrentDomain.Load("VetCare.Domain");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblyDomain));

        var assemblyApplication = AppDomain.CurrentDomain.Load("VetCare.Application");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblyApplication));

        var assemblyCrossCutting = AppDomain.CurrentDomain.Load("VetCare.Infrastructure.CrossCutting");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblyCrossCutting));
        

        // 4. Serviços auxiliares (HashId, JWT, Clock, etc.) -------
        services.AddSingleton<IHashIdService, HashIdService>();
  
        // 5. Behaviors (Validation, Tenant, Logging) --------------
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TenantAuthorizationBehavior<,>));
        return services;
    }
}
