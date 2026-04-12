using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class modifiedrolevariable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissionLevel",
                table: "Accounts");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Accounts");

            migrationBuilder.AddColumn<string>(
                name: "PermissionLevel",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
