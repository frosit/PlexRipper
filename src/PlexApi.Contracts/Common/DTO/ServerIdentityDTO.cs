namespace PlexApi.Contracts;

public record ServerIdentityDTO
{
    public required bool Claimed { get; init; }

    public required string MachineIdentifier { get; init; }

    public required string Version { get; init; }
}
