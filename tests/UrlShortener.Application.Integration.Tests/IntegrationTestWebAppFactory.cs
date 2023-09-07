using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;
using UrlShortener.Api;
using UrlShortener.Application.Infrastructure.Persistence;
using UrlShortener.Application.Services;
using Xunit;

namespace UrlShortener.Application.Integration.Tests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithEnvironment("ACCEPT_EULA=Y", "SA_PASSWORD=$TrongP@4assw0rd")
        .WithHostname("localhost")
        .WithName("urlshortener-db")
        .Build();

    public Task InitializeAsync()
    {
        return _msSqlContainer.StartAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(s => s.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(_msSqlContainer.GetConnectionString()));

            // Ensuring that the database is created and all migrations are applied.
            // Could also uncomment the ApplyMigrations extension method in Program.cs.
            var db = services.BuildServiceProvider().GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        });
    }

    public new Task DisposeAsync()
    {
        return _msSqlContainer.StopAsync();
    }
}
