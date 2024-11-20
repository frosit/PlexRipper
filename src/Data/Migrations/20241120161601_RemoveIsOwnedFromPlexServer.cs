using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsOwnedFromPlexServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Owned", table: "PlexServers");

            migrationBuilder.DropColumn(name: "ServerFixApplyDNSFix", table: "PlexServers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<bool>(
                    name: "Owned",
                    table: "PlexServers",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 16);

            migrationBuilder
                .AddColumn<bool>(
                    name: "ServerFixApplyDNSFix",
                    table: "PlexServers",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 25);
        }
    }
}
