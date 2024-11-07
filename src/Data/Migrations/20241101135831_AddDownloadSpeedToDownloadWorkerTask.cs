using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDownloadSpeedToDownloadWorkerTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<long>(
                    name: "DownloadSpeed",
                    table: "DownloadWorkerTasks",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: 0L
                )
                .Annotation("Relational:ColumnOrder", 11);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "DownloadSpeed", table: "DownloadWorkerTasks");
        }
    }
}
