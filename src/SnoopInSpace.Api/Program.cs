using SnoopInSpace.Api.DependencyInjection;
using SnoopInSpace.Adapters.InMemory.DependencyInjection;
using SnoopInSpace.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
builder.Services.AddInMemoryAdapters();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet(
    "/ping",
    () => Results.Ok());

app.MapAuthEndpoints();

app.Run();

public partial class Program;