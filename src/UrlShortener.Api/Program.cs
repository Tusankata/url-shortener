using Carter;
using Microsoft.OpenApi.Models;
using Serilog;
using UrlShortener.Api.Properties.Middlewares;
using UrlShortener.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration));


builder.Services.AddCarter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(o => o.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new OpenApiInfo { Title = "URL Shortener API", Version = "v1" }));

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

builder.Services.AddProblemDetails();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

//app.ApplyMigrations();

app.UseGlobalExceptionHandlingMiddleware();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    o.RoutePrefix = string.Empty;
});


app.UseCors();
app.UseHttpsRedirection();
app.MapCarter();
app.UseRouting();

app.Run();