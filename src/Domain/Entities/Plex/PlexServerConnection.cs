using System.ComponentModel.DataAnnotations.Schema;

namespace PlexRipper.Domain;

/// <summary>
/// Every <see cref="PlexServer"/> might have different ways to setup a connection through various domains or ip addresses.
/// All possible connections are stored through the use of <see cref="PlexServerConnection">PlexServerConnections</see>.
/// </summary>
public class PlexServerConnection : BaseEntity
{
    #region Properties

    [Column(Order = 1)]
    public required string Protocol { get; init; }

    [Column(Order = 2)]
    public required string Address { get; init; }

    [Column(Order = 3)]
    public required int Port { get; init; }

    [Column(Order = 4)]
    public required string Uri { get; init; }

    [Column(Order = 5)]
    public required bool Local { get; init; }

    [Column(Order = 6)]
    public required bool Relay { get; init; }

    [Column(Order = 7)]
    public required bool IPv4 { get; init; }

    [Column(Order = 8)]
    public required bool IPv6 { get; init; }

    /// <summary>
    /// Is this a custom connection created by the user.
    /// </summary>
    [Column(Order = 9)]
    public required bool IsCustom { get; init; }

    #endregion

    #region Relationships

    public PlexServer? PlexServer { get; init; }

    public required int PlexServerId { get; set; }

    public List<PlexServerStatus> PlexServerStatus { get; init; } = [];

    #endregion

    #region Helpers

    [NotMapped]
    public string Url
    {
        get
        {
            var urlBuilder = new UriBuilder(Protocol, Address) { Port = Port };
            return urlBuilder.ToString().TrimEnd('/');
        }
    }

    [NotMapped]
    public string Name => $"Connection: ({Url})";

    [NotMapped]
    public PlexServerStatus? LatestConnectionStatus => PlexServerStatus.FirstOrDefault();

    [NotMapped]
    public bool IsOnline => LatestConnectionStatus?.IsSuccessful ?? false;

    [NotMapped]
    public bool IsPlexTvConnection => Uri.Contains(".plex.direct");

    public string GetThumbUrl(string thumbPath)
    {
        var uri = new Uri(Url + thumbPath);
        return $"{uri.Scheme}://{uri.Host}:{uri.Port}/photo/:/transcode?url={uri.AbsolutePath}";
    }

    public string GetDownloadUrl(string fileLocationUrl, string token) =>
        $"{Url}{fileLocationUrl}?X-Plex-Token={token}";

    #endregion

    #region Operators

    public static bool operator ==(PlexServerConnection left, PlexServerConnection right) => Equals(left, right);

    public static bool operator !=(PlexServerConnection left, PlexServerConnection right) => !Equals(left, right);

    #endregion

    #region Equality

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Protocol, Address, Port, Uri, Local, Relay, IPv4, IPv6);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        return obj.GetType() == GetType() && Equals((PlexServerConnection)obj);
    }

    protected bool Equals(PlexServerConnection other) =>
        Protocol == other.Protocol
        && Address == other.Address
        && Port == other.Port
        && Uri == other.Uri
        && Local == other.Local
        && Relay == other.Relay
        && IPv6 == other.IPv6;

    #endregion

    /// <inheritdoc/>
    public override string ToString() =>
        $"[ServerId: {PlexServerId} - Url: {Url} - Local: {Local} - Relay: {Relay} - IPv6: {IPv6}]";
}
