using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using VetCare.Infrastructure.CrossCutting.Extensions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle



builder.Services.AddCrossCutting(builder.Configuration);
builder.Services.AddVetCareCore(builder.Configuration);

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;

    opt.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"));
})
.AddApiExplorer(opt => {
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;

});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        Name = "Authorization",
        Description = "Digite: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    { 
        { 
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }                
            },new List<string>() 
        }
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(opt =>
    {
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            opt.SwaggerEndpoint
            (
                $"/swagger/{desc.GroupName}/swagger.json",
                $"VetCare {desc.GroupName.ToUpperInvariant()}"
            );
        }
    });
}

app.UseHttpsRedirection();
app.UseCrossCutting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();

app.MapControllers();

app.Run();
