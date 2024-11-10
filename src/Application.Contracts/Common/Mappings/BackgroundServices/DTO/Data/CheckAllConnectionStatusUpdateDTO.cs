namespace Application.Contracts;

public record CheckAllConnectionStatusUpdateDTO
{
    public required Dictionary<int, List<int>> PlexServersWithConnectionIds { get; init; } = new();
}
