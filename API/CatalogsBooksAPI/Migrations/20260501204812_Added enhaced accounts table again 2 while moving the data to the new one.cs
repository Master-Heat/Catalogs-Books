using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedenhacedaccountstableagain2whilemovingthedatatothenewone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. First, Add the new columns
            migrationBuilder.AddColumn<string>(
                name: "AccountState",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            // 2. DATA MOVE: Use SQL to transform IsAdmin (bit) into Role (string)
            // We do this while IsAdmin still exists!
            migrationBuilder.Sql(
                @"UPDATE Accounts 
          SET Role = CASE WHEN IsAdmin = 1 THEN 'Admin' ELSE 'User' END,
              AccountState = 'Active'");

            // 3. NOW it is safe to drop the old column
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Accounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert logic: Bring back IsAdmin
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // Convert string roles back to booleans
            migrationBuilder.Sql(
                @"UPDATE Accounts 
          SET IsAdmin = CASE WHEN Role = 'Admin' THEN 1 ELSE 0 END");

            migrationBuilder.DropColumn(
                name: "AccountState",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Accounts");
        }
    }
}
