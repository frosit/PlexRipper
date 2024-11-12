using System.Net;
using System.Net.Sockets;
using Application.Contracts;
using Data.Contracts;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace PlexRipper.Application;

public record CreatePlexServerConnectionEndpointRequest()
{
    public string Url { get; init; }

    public string Protocol { get; init; }

    public string Address { get; init; }

    public int Port { get; init; }

    public int PlexServerId { get; set; }
};

public class CreatePlexServerConnectionEndpointRequestValidator : Validator<CreatePlexServerConnectionEndpointRequest>
{
    public CreatePlexServerConnectionEndpointRequestValidator()
    {
        RuleFor(x => x.Url).NotEmpty();
        RuleFor(x => x.Protocol).NotEmpty();
        RuleFor(x => x.Address).NotEmpty();
        RuleFor(x => x.Port).GreaterThan(0);
    }
}

public class CreatePlexServerConnectionEndpoint
    : BaseEndpoint<CreatePlexServerConnectionEndpointRequest, ResultDTO<PlexServerConnectionDTO>>
{
    private readonly IPlexRipperDbContext _dbContext;

    public override string EndpointPath => ApiRoutes.PlexServerConnectionController;

    public CreatePlexServerConnectionEndpoint(IPlexRipperDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Post(EndpointPath);
        AllowAnonymous();
        Description(x =>
            x.ClearDefaultProduces()
                .Produces(StatusCodes.Status201Created, typeof(ResultDTO<PlexServerConnectionDTO>))
                .Produces(StatusCodes.Status400BadRequest, typeof(ResultDTO))
                .Produces(StatusCodes.Status500InternalServerError, typeof(ResultDTO))
        );
    }

    public override async Task HandleAsync(CreatePlexServerConnectionEndpointRequest req, CancellationToken ct)
    {
        var connection = new PlexServerConnection
        {
            Id = 0,
            Protocol = req.Protocol.ToLower(),
            Address = req.Address,
            Port = req.Port,
            Uri = req.Url,
            Local = IsLocalUrl(req.Address),
            Relay = req.Address.Contains("plex.direct"),
            IPv4 = IsIpv4(req.Address),
            IPv6 = IsIPv6(req.Address),
            PlexServerId = req.PlexServerId,
            IsCustom = true,
        };
        _dbContext.PlexServerConnections.Add(connection);

        await _dbContext.SaveChangesAsync(ct);

        var result = ResultExtensions.Create200OkResult(connection.ToDTO());
        await SendFluentResult(result, ct);
    }

    private bool IsLocalUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false; // Invalid URL
        }

        // Check if the hostname is "localhost" or "127.0.0.1" (IPv4 loopback)
        if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) || uri.Host.Equals("127.0.0.1"))
        {
            return true;
        }

        // Check if it's an IP address
        if (IPAddress.TryParse(uri.Host, out var ipAddress))
        {
            // Check if it's an IPv4 private or loopback address
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                var bytes = ipAddress.GetAddressBytes();
                return bytes[0] == 10
                    || // 10.x.x.x
                    (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                    || // 172.16.x.x - 172.31.x.x
                    (bytes[0] == 192 && bytes[1] == 168)
                    || // 192.168.x.x
                    ipAddress.Equals(IPAddress.Loopback); // 127.0.0.1
            }

            // Check if it's an IPv6 local or loopback address
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                return ipAddress.IsIPv6LinkLocal
                    || // fe80::/10 range
                    ipAddress.IsIPv6SiteLocal
                    || // fec0::/10 range (deprecated but sometimes still used)
                    ipAddress.Equals(IPAddress.IPv6Loopback); // ::1
            }
        }

        return false;
    }

    private bool IsIpv4(string address)
    {
        if (IPAddress.TryParse(address, out var ipAddress))
        {
            return ipAddress.AddressFamily == AddressFamily.InterNetwork;
        }

        return false;
    }

    private bool IsIPv6(string address)
    {
        if (IPAddress.TryParse(address, out var ipAddress))
        {
            return ipAddress.AddressFamily == AddressFamily.InterNetworkV6;
        }

        return false;
    }
}
