﻿namespace PlexRipper.Application;

public interface IPlexServerService
{
    /// <summary>
    /// Retrieves the latest accessible <see cref="PlexServer">PlexServers</see> for this <see cref="PlexAccount"/> from the PlexAPI and stores it in the Database.
    /// </summary>
    /// <param name="plexAccountId">The id of the <see cref="PlexAccount"/> to check.</param>
    /// <returns>Is successful.</returns>
    Task<Result<List<PlexServer>>> RefreshAccessiblePlexServersAsync(int plexAccountId);

    Task<Result<PlexServer>> GetServerAsync(int plexServerId);

    /// <summary>
    /// Retrieves all <see cref="PlexServer"/>s from the Database with the included <see cref="PlexLibrary"/>.
    /// </summary>
    /// <param name="includeLibraries">Include the nested <see cref="PlexLibrary">PlexLibraries</see>.</param>
    /// <returns>The list of <see cref="PlexServer">PlexServers</see>.</returns>
    Task<Result<List<PlexServer>>> GetAllPlexServersAsync(bool includeLibraries = false);

    /// <summary>
    /// Will inspect all <see cref="PlexServer">PlexServers</see> added to this <see cref="PlexAccount"/>
    /// and checks its connectivity status and which libraries can be accessed.
    /// </summary>
    /// <param name="plexAccountId"></param>
    /// <param name="skipRefreshAccessibleServers"></param>
    /// <returns></returns>
    Task<Result> InspectPlexServers(int plexAccountId, bool skipRefreshAccessibleServers = false);

    Task<Result> SyncPlexServers(bool forceSync = false);

    Task<Result> SyncPlexServers(List<int> plexServerIds, bool forceSync = false);

    /// <summary>
    /// Take all <see cref="PlexLibrary">PlexLibraries.</see> and retrieve all data to then store in the database.
    /// </summary>
    /// <param name="plexServerId">The id of the <see cref="PlexServer"/> to use.</param>
    /// <param name="forceSync">By default, the libraries which have been synced less than 6 hours ago will be skipped. </param>
    /// <returns><see cref="Result"/></returns>
    Task<Result> SyncPlexServer(int plexServerId, bool forceSync = false);

    Task<Result<PlexServer>> RefreshAccessiblePlexServerAsync(int plexServerId);

    Task<Result<PlexServer>> InspectPlexServer(int plexServerId);

    Task<Result> SetPreferredConnection(int plexServerId, int plexServerConnectionId);
}