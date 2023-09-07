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
using UrlShortener.Application.Common.Exceptions;
using UrlShortener.Application.Entities;
using UrlShortener.Application.Infrastructure.Persistence;

namespace UrlShortener.Application.Features;

public class GetEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/{code}", async (
            [FromRoute] string code,
            ISender sender,
            IValidator<GetShortUrlQuery> validator) =>
            {
                var request = new GetShortUrlQuery(code);

                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var result = await sender.Send(request);
                return Results.Redirect(result.LongUrl);
            })
            .WithName("GetShortUrl")
            .WithTags("URL Shortening")
            .Produces(StatusCodes.Status302Found)
            .Produces<ValidationProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .IncludeInOpenApi();
    }
}

public class GetShortUrlQueryValidator : AbstractValidator<GetShortUrlQuery>
{
    public GetShortUrlQueryValidator()
    {
        RuleFor(query => query.Code)
            .NotEmpty()
            .WithMessage("The short code cannot be empty.")
            .NotNull()
            .WithMessage("The short code cannot be null.")
            .Must(code => code.Length == 7)
            .WithMessage("The short code must be 7 characters long.");
    }
}

public record GetShortUrlQuery(string Code) : IRequest<LongUrlResponse>;

internal sealed class GetShortUrlHandler : IRequestHandler<GetShortUrlQuery, LongUrlResponse>
{
    private readonly AppDbContext _context;

    public GetShortUrlHandler(AppDbContext context) => _context = context;

    public async Task<LongUrlResponse> Handle(GetShortUrlQuery request, CancellationToken cancellationToken)
    {
        var code = request.Code;
        var url = await _context.ShortenedUrls.FirstOrDefaultAsync(url => url.ShortCode == code, cancellationToken)
            ?? throw new NotFoundException($"The short code {code} was not found.");
        url.IncrementClicks();
        _context.Update(url);
        await _context.SaveChangesAsync(cancellationToken);

        return new LongUrlResponse(url.LongUrl);
    }
}

public record LongUrlResponse(string LongUrl);