using Carter;
using Carter.OpenApi;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using UrlShortener.Application.Common.Extensions;
using UrlShortener.Application.Common.Models.Enums;
using UrlShortener.Application.Entities;
using UrlShortener.Application.Features.Common;
using UrlShortener.Application.Infrastructure.Persistence;

namespace UrlShortener.Application.Features;

public class CreateShortUrlModule : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/urls", async (
            CreateShortUrlRequest request,
            ISender sender,
            IValidator<CreateShortUrlRequest> validator,
            HttpContext context) =>
            {
                var validationResult = await validator.ValidateAsync(request);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await sender.Send(new CreateShortUrlCommand(request,
                    context.Request.Scheme,
                    context.Request.Host.ToString()));
                return Results.CreatedAtRoute("GetShortUrl", new { Code = result.ShortCode }, result.ShortUrl);
            })
            .Accepts<CreateShortUrlRequest>("application/json")
            .WithDisplayName("ShortenUrl")
            .WithTags("URL Shortening")
            .Produces(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
            .IncludeInOpenApi();
    }
}

public record CreateShortUrlRequest(string Url);

public record CreateShortUrlCommand(CreateShortUrlRequest Request, string Scheme, string Host) : IRequest<UrlResponse>;

public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlRequest>
{
    public CreateShortUrlValidator()
    {
        RuleFor(request => request.Url)
            .NotEmpty()
            .WithMessage("You must provide a URL.")
            .Must(BeAValidUrl)
            .WithMessage("You must provide a valid URL.");
    }

    private static bool BeAValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out _);
}

internal sealed class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, UrlResponse>
{
    private readonly AppDbContext _context;

    public CreateShortUrlHandler(AppDbContext context) => _context = context;

    public async Task<UrlResponse> Handle(CreateShortUrlCommand command, CancellationToken cancellationToken)
    {
        var availableCode = await _context.ShortUrlCodes.FirstOrDefaultAsync(
            code => code.State == CodeState.Available, cancellationToken);

        var shortUrl = ShortenedUrl.Create(
            command.Request.Url,
            $"{command.Scheme}://{command.Host}/api/{availableCode}",
            availableCode!.ToString()
        );

        availableCode.MarkAsUsed();
        _context.Update(availableCode);

        _context.Add(shortUrl);

        await _context.SaveChangesAsync(cancellationToken);

        return shortUrl.ToResponse();
    }
}