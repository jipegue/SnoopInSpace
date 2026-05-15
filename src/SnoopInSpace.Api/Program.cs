using Microsoft.OpenApi;
using SnoopInSpace.Adapters.InMemory.DependencyInjection;
using SnoopInSpace.Api.DependencyInjection;
using SnoopInSpace.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token"
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer", document),
                []
            }
        });
});

builder.Services.AddApplicationServices();
builder.Services.AddInMemoryAdapters();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet(
    "/ping",
    () => Results.Ok());


app.MapAuthEndpoints();

app.Run();

public partial class Program;