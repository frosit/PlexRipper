using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidTypesToPlexMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid_IMDB",
                table: "PlexTvShowSeason",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TMDB",
                table: "PlexTvShowSeason",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TVDB",
                table: "PlexTvShowSeason",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 26);

            migrationBuilder.AddColumn<string>(
                name: "Guid_IMDB",
                table: "PlexTvShows",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TMDB",
                table: "PlexTvShows",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TVDB",
                table: "PlexTvShows",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 26);

            migrationBuilder.AddColumn<string>(
                name: "Guid_IMDB",
                table: "PlexTvShowEpisodes",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TMDB",
                table: "PlexTvShowEpisodes",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TVDB",
                table: "PlexTvShowEpisodes",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 26);

            migrationBuilder.AddColumn<string>(
                name: "Guid_IMDB",
                table: "PlexMovie",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 24);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TMDB",
                table: "PlexMovie",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 25);

            migrationBuilder.AddColumn<string>(
                name: "Guid_TVDB",
                table: "PlexMovie",
                type: "TEXT",
                nullable: true)
                .Annotation("Relational:ColumnOrder", 26);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid_IMDB",
                table: "PlexTvShowSeason");

            migrationBuilder.DropColumn(
                name: "Guid_TMDB",
                table: "PlexTvShowSeason");

            migrationBuilder.DropColumn(
                name: "Guid_TVDB",
                table: "PlexTvShowSeason");

            migrationBuilder.DropColumn(
                name: "Guid_IMDB",
                table: "PlexTvShows");

            migrationBuilder.DropColumn(
                name: "Guid_TMDB",
                table: "PlexTvShows");

            migrationBuilder.DropColumn(
                name: "Guid_TVDB",
                table: "PlexTvShows");

            migrationBuilder.DropColumn(
                name: "Guid_IMDB",
                table: "PlexTvShowEpisodes");

            migrationBuilder.DropColumn(
                name: "Guid_TMDB",
                table: "PlexTvShowEpisodes");

            migrationBuilder.DropColumn(
                name: "Guid_TVDB",
                table: "PlexTvShowEpisodes");

            migrationBuilder.DropColumn(
                name: "Guid_IMDB",
                table: "PlexMovie");

            migrationBuilder.DropColumn(
                name: "Guid_TMDB",
                table: "PlexMovie");

            migrationBuilder.DropColumn(
                name: "Guid_TVDB",
                table: "PlexMovie");
        }
    }
}
