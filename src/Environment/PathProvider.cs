using System;
using System.Reflection;

namespace Environment;

public class PathProvider : IPathProvider
{
    #region Properties

    #region DirectoryNames

    private static readonly string _configFolder = "Config";

    private static readonly string _logsFolder = "Logs";

    public static string DefaultMovieDestinationFolder => "Movies";

    public static string DefaultDownloadsDestinationFolder => "Downloads";

    public static string DefaultTvShowsDestinationFolder => "TvShows";

    public static string DefaultMusicDestinationFolder => "Music";

    public static string DefaultPhotosDestinationFolder => "Photos";

    public static string DefaultOtherDestinationFolder => "Other";

    public static string DefaultGamesDestinationFolder => "Games";

    #endregion

    #region FileNames

    public static string ConfigFileName => "PlexRipperSettings.json";

    public static string DatabaseName => "PlexRipperDB.db";
    public static string DatabaseShmName => $"{DatabaseName}-shm";
    public static string DatabaseWalName => $"{DatabaseName}-wal";

    #endregion

    #region EnvVarKeys

    // The Plexripper env. Could be ENV=prod or ENV=dev or ENV=CI
    // Then we can use this to alter behaviour
    // @todo this is the same as DOTNET_ENVIRONMENT in Dockerfile
    public static string EnvKeyPlexRipperEnvironment => "PLEXRIPPER_ENV";

    public static string EnvKeyPlexRipperVersion => "PLEXRIPPER_VERSION";

    // To allow prepending all paths with a path, for example use the user "$home/PlexRipper/" as path instead of "/"
    public static string EnvKeyPlexRipperRoot => "PLEXRIPPER_ROOT";

    // the document root for static nuxt assets, served by a webserver, e.g. /var/www/html, for configuring webserver
    public static string EnvKeyPlexRipperDocRoot => "PLEXRIPPER_DOCROOT";

    // From Dockerfile
    // @todo utilize or refactor
    public static string EnvKeyPlexRipperNuxtHost => "PLEXRIPPER_NUXT_HOST";
    public static string EnvKeyPlexRipperNuxtPort => "PLEXRIPPER_NUXT_PORT";
    public static string EnvKeyPlexRipperApiPort => "PLEXRIPPER_API_PORT";
    public static string EnvKeyPlexRipperNuxtPublicIsDocker => "PLEXRIPPER_NUXT_PUBLIC_IS_DOCKER";

    #endregion

    public static string ConfigDirectory => Path.Combine(RootDirectory, _configFolder);

    public static string ConfigFileLocation => Path.Join(ConfigDirectory, ConfigFileName);

    public static string DatabaseBackupDirectory => Path.Combine(ConfigDirectory, "Database BackUp");

    public static string DatabasePath => Path.Combine(ConfigDirectory, DatabaseName);

    public static string Database_SHM_Path => Path.Combine(ConfigDirectory, DatabaseShmName);

    public static string Database_WAL_Path => Path.Combine(ConfigDirectory, DatabaseWalName);

    public static string LogsDirectory => Path.Combine(RootDirectory, _configFolder, _logsFolder);

    /**
     * Get Environment variable where PLEXRIPPER_ prefixed keys rule
     * Accepts a ENV VAR key, prefereably from EnvVarKeys region
     *
     * @todo @jasonlandbridge : will this work if the if statement also is null? because public static string?
     * @todo write test
     * @todo add variables to debug
     */
    public static string GetPlexRipperEnvVar(string key)
    {
        string value = System.Environment.GetEnvironmentVariable(key);

        // If the key is not defined, remove "PLEXRIPPER_" prefix and check again
        if (string.IsNullOrEmpty(value) && key.StartsWith("PLEXRIPPER_"))
        {
            key = key.Substring("PLEXRIPPER_".Length);
            value = System.Environment.GetEnvironmentVariable(key);
        }

        return value;
    }

    public static string RootDirectory
    {
        get
        {
            var devRootPath = EnvironmentExtensions.GetDevelopmentRootPath();
            if (devRootPath is not null)
                return devRootPath;

            // @todo detect docker env
            switch (OsInfo.CurrentOS)
            {
                case OperatingSystemPlatform.Linux:
                case OperatingSystemPlatform.Osx:
                    var rootDir = "/app"; // default
                    var homeDirectory = System.Environment.GetEnvironmentVariable("HOME");
                    var plexRipperRoot = System.Environment.GetEnvironmentVariable(EnvKeyPlexRipperRoot);

                    // If PLEXRIPPER_ROOT is defined, set that path as root
                    if (!string.IsNullOrEmpty(plexRipperRoot))
                    {
                        // Create dir if not exists
                        if (!System.IO.Directory.Exists(plexRipperRoot))
                        {
                            Directory.CreateDirectory(plexRipperRoot);
                        }
                        return plexRipperRoot;
                    }

                    // @todo detect docker env, maybe set it to /app or something

                    // If we know the Home dir, set root to a subdir of home
                    // @todo in a docker env, we may want to set it to /app or something
                    if (!string.IsNullOrEmpty(homeDirectory))
                    {
                        rootDir = Path.Combine(homeDirectory, ".PlexRipper"); // subdir
                    }
                    else
                    {
                        rootDir = Path.Combine("/", "app");
                    }

                    // Check dir existence before return
                    // @todo should do a try catch? in case we don't have permission to create, or fallback to tmp/?
                    if (!System.IO.Directory.Exists(rootDir))
                    {
                        Directory.CreateDirectory(rootDir);
                    }

                    return rootDir;
                case OperatingSystemPlatform.Windows:
                    // @todo this should be somewhere in AppData/Local/PlexRipper ?
                    return Path.GetPathRoot(Assembly.GetExecutingAssembly().Location) ?? @"C:\";
                default:
                    // @todo do we want this? it requires /Config, better make it /app or something at least
                    return "/";
            }
        }
    }

    public static string DatabaseConnectionString => $"Data Source={DatabasePath}";

    // @todo do we have to extend this with net methods implemented?

    #region Interface Implementations

    string IPathProvider.RootDirectory => RootDirectory;

    string IPathProvider.ConfigFileLocation => ConfigFileLocation;

    string IPathProvider.ConfigFileName => ConfigFileName;

    string IPathProvider.DatabaseBackupDirectory => DatabaseBackupDirectory;

    string IPathProvider.DatabaseName => DatabaseName;

    string IPathProvider.DatabasePath => DatabasePath;

    string IPathProvider.Database_SHM_Path => Database_SHM_Path;

    string IPathProvider.Database_WAL_Path => Database_WAL_Path;

    string IPathProvider.LogsDirectory => LogsDirectory;

    string IPathProvider.ConfigDirectory => ConfigDirectory;

    public List<string> DatabaseFiles => new()
    {
        DatabasePath,
        Database_SHM_Path,
        Database_WAL_Path,
    };

    #endregion

    #endregion
}