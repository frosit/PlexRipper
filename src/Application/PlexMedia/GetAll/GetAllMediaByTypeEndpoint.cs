using System.ComponentModel;
using Application.Contracts;
using Data.Contracts;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace PlexRipper.Application;

public record GetAllMediaByTypeRequest
{
    public PlexMediaType MediaType { get; init; }

    [QueryParam]
    [DefaultValue(0)]
    public int Page { get; init; }

    [QueryParam]
    [DefaultValue(0)]
    public int Size { get; init; }
}

public class GetAllMediaByTypeRequestValidator : Validator<GetAllMediaByTypeRequest>
{
    public GetAllMediaByTypeRequestValidator()
    {
        RuleFor(x => x.MediaType)
            .Must(type => type is PlexMediaType.TvShow or PlexMediaType.Movie)
            .WithMessage(x => $"Media type {x.MediaType} is not allowed.");
        RuleFor(x => x.Page).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Size).GreaterThanOrEqualTo(0);
    }
}

public class GetAllMediaByTypeEndpoint : BaseEndpoint<GetAllMediaByTypeRequest, List<PlexMediaSlimDTO>>
{
    private readonly IPlexRipperDbContext _dbContext;

    public override string EndpointPath => ApiRoutes.PlexMediaController;

    public GetAllMediaByTypeEndpoint(IPlexRipperDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Get(EndpointPath);
        AllowAnonymous();
        Description(x =>
            x.Produces(StatusCodes.Status200OK, typeof(ResultDTO<List<PlexMediaSlimDTO>>))
                .Produces(StatusCodes.Status400BadRequest, typeof(ResultDTO))
                .Produces(StatusCodes.Status500InternalServerError, typeof(ResultDTO))
        );
    }

    public override async Task HandleAsync(GetAllMediaByTypeRequest req, CancellationToken ct)
    {
        // When 0, just take everything
        var take = req.Size <= 0 ? 0 : req.Size;
        var skip = req.Page * req.Size;

        var entitiesResult = await _dbContext.GetMediaByType(req.MediaType, skip, take, ct: ct);

        await SendFluentResult(entitiesResult, ct);
    }
}
