using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UrlShortener.Application.Behaviours;
using UrlShortener.Application.Common.Exceptions.Handlers;
using UrlShortener.Application.Common.Models.Abstractions;
using UrlShortener.Application.Entities;
using UrlShortener.Application.Infrastructure;
using UrlShortener.Application.Infrastructure.Persistence;
using UrlShortener.Application.Services;

namespace UrlShortener.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(o => o.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));
        services.AddHostedService<CodeGenerationBackgroundService>();
        services.AddTransient<ExceptionHandlerFactory>();
        services.AddTransient<NotFoundExceptionHandler>();
        services.AddTransient<DefaultExceptionHandler>();
        services.AddScoped<ICodeDisposer, CodeDisposer>();
        services.AddScoped<ICodeGenerator, CodeGenerator>();
        services.AddScoped<ICodeProcessor, CodeProcessor>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        bool useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase");

        if (useInMemoryDatabase)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("UrlShortenerDb");
            });
        }
        else
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
            });
        }

        return services;
    }

    public static IApplicationBuilder ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        return app;
    }
}
