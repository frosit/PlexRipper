using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePortFixFromPlexServerConnections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PortFix", table: "PlexServerConnections");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<bool>(
                    name: "PortFix",
                    table: "PlexServerConnections",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 9);
        }
    }
}
