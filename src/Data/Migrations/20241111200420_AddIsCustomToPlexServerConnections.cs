using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCustomToPlexServerConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<bool>(
                    name: "IsCustom",
                    table: "PlexServerConnections",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 9);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsCustom", table: "PlexServerConnections");
        }
    }
}
