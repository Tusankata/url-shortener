using Xunit;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using UrlShortener.Application.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UrlShortener.Application.Services;
using Microsoft.Extensions.Hosting;

namespace UrlShortener.Application.Integration.Tests;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly ISender Sender;
    private readonly IServiceScope _scope;
    protected readonly AppDbContext DbContext;
    protected readonly IEnumerable<IHostedService> HostedServices;
    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
        HostedServices = _scope.ServiceProvider.GetServices<IHostedService>();
    }
}
