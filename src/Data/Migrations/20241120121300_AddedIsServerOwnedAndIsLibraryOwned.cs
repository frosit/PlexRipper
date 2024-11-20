using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlexRipper.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsServerOwnedAndIsLibraryOwned : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .AddColumn<bool>(
                    name: "IsServerOwned",
                    table: "PlexAccountServers",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 4);

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexServerId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .Annotation("Relational:ColumnOrder", 2);

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexLibraryId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexAccountId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder
                .AddColumn<bool>(
                    name: "IsLibraryOwned",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    defaultValue: false
                )
                .Annotation("Relational:ColumnOrder", 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsServerOwned", table: "PlexAccountServers");

            migrationBuilder.DropColumn(name: "IsLibraryOwned", table: "PlexAccountLibraries");

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexServerId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .OldAnnotation("Relational:ColumnOrder", 2);

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexLibraryId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder
                .AlterColumn<int>(
                    name: "PlexAccountId",
                    table: "PlexAccountLibraries",
                    type: "INTEGER",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "INTEGER"
                )
                .OldAnnotation("Relational:ColumnOrder", 0);
        }
    }
}
