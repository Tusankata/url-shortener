using FluentAssertions;
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Application.Features;
using Xunit;

namespace UrlShortener.Application.Integration.Tests.UrlTests;

public class UrlTests : BaseIntegrationTest
{
    public UrlTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GivenValid_CreateShortUrlCommand_ShouldCreateShortUrl()
    {
        // Arrange
        await Task.Delay(TimeSpan.FromSeconds(5));
        var command = new CreateShortUrlCommand(
            new CreateShortUrlRequest("https://www.google.com"),
            "http",
            "localhost");

        var result = await Sender.Send(command);

        // Assert
        var url = DbContext.ShortenedUrls.FirstOrDefault(p => p.Id == result.UrlId);

        url.Should().NotBeNull();
        url!.Id.Should().Be(result.UrlId);
        url!.ShortCode.Should().Be(result.ShortCode);
    }

    [Fact]
    public async Task GivenValid_GetShortUrlQuery_ShouldReturnLongUrl()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        var command = new CreateShortUrlCommand(
            new CreateShortUrlRequest("https://www.google.com"),
            "http",
            "localhost");

        var shortened = await Sender.Send(command);

        var query = new GetShortUrlQuery(shortened.ShortCode);

        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result!.LongUrl.Should().Be(shortened.OriginalUrl);
    }

    [Fact]
    public async Task GivenInvalid_GetShortUrlQuery_ShouldThrowNotFoundException()
    {
        await Task.Delay(TimeSpan.FromSeconds(5));
        var query = new GetShortUrlQuery("invalid");

        // Act
        Func<Task> action = async () => await Sender.Send(query);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage("The short code invalid was not found.");
    }
}
