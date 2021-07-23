﻿using Microsoft.EntityFrameworkCore;
using PlexRipper.Domain;

namespace PlexRipper.Data
{
    public static class PlexRipperDBContextSeed
    {
        public static ModelBuilder SeedDatabase(ModelBuilder builder)
        {
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 1, DisplayName = "Download Path", DirectoryPath = "/downloads", FolderType = FolderType.DownloadFolder });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 2, DisplayName = "Movie Destination Path", DirectoryPath = "/movies", FolderType = FolderType.MovieFolder });
            builder.Entity<FolderPath>().HasData(
                new FolderPath
                {
                    Id = 3, DisplayName = "Tv Show Destination Path", DirectoryPath = "/tvshows", FolderType = FolderType.TvShowFolder,
                });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 4, DisplayName = "Music Destination Path", DirectoryPath = "/music", FolderType = FolderType.MusicFolder });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 5, DisplayName = "Photos Destination Path", DirectoryPath = "/photos", FolderType = FolderType.PhotosFolder });
            builder.Entity<FolderPath>().HasData(
                new FolderPath
                {
                    Id = 6, DisplayName = "Other Videos Destination Path", DirectoryPath = "/other", FolderType = FolderType.OtherVideosFolder,
                });
            builder.Entity<FolderPath>().HasData(
                new FolderPath
                {
                    Id = 7, DisplayName = "Games Videos Destination Path", DirectoryPath = "/games", FolderType = FolderType.GamesVideosFolder,
                });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 8, DisplayName = "Reserved #1 Destination Path", DirectoryPath = "/", FolderType = FolderType.None });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 9, DisplayName = "Reserved #2 Destination Path", DirectoryPath = "/", FolderType = FolderType.None });
            builder.Entity<FolderPath>().HasData(
                new FolderPath { Id = 10, DisplayName = "Reserved #3 Destination Path", DirectoryPath = "/", FolderType = FolderType.None });

            return builder;
        }
    }
}