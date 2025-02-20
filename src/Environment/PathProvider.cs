﻿using System.Reflection;

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

    public static string ConfigDirectory => Path.Combine(RootDirectory, _configFolder);

    public static string ConfigFileLocation => Path.Join(ConfigDirectory, ConfigFileName);

    public static string DatabaseBackupDirectory => Path.Combine(ConfigDirectory, "Database BackUp");

    public static string DatabasePath => Path.Combine(ConfigDirectory, DatabaseName);

    public static string Database_SHM_Path => Path.Combine(ConfigDirectory, DatabaseShmName);

    public static string Database_WAL_Path => Path.Combine(ConfigDirectory, DatabaseWalName);

    public static string LogsDirectory => Path.Combine(RootDirectory, _configFolder, _logsFolder);

    public List<string> DatabaseFiles => [DatabasePath, Database_SHM_Path, Database_WAL_Path];

    public static string RootDirectory
    {
        get
        {
            var devRootPath = EnvironmentExtensions.GetDevelopmentRootPath();
            if (devRootPath is not null)
                return devRootPath;

            switch (OsInfo.CurrentOS)
            {
                case OperatingSystemPlatform.Linux:
                case OperatingSystemPlatform.Osx:
                    return "/";
                case OperatingSystemPlatform.Windows:
                    return Path.GetPathRoot(Assembly.GetExecutingAssembly().Location) ?? @"C:\";
                default:
                    return "/";
            }
        }
    }

    #region Interface Implementations

    string IPathProvider.RootDirectory => RootDirectory;

    string IPathProvider.ConfigFileLocation => ConfigFileLocation;

    string IPathProvider.ConfigFileName => ConfigFileName;

    string IPathProvider.DatabaseBackupDirectory => DatabaseBackupDirectory;

    string IPathProvider.DatabaseName => DatabaseName;

    string IPathProvider.DatabasePath => DatabasePath;

    string IPathProvider.LogsDirectory => LogsDirectory;

    string IPathProvider.ConfigDirectory => ConfigDirectory;

    #endregion

    #endregion
}
