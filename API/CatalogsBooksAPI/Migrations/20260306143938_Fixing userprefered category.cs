using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class Fixinguserpreferedcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedCategories_Accounts_AccountID1",
                table: "UserPreferedCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedCategories_AccountID1",
                table: "UserPreferedCategories");

            migrationBuilder.DropColumn(
                name: "AccountID1",
                table: "UserPreferedCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountID1",
                table: "UserPreferedCategories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_AccountID1",
                table: "UserPreferedCategories",
                column: "AccountID1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedCategories_Accounts_AccountID1",
                table: "UserPreferedCategories",
                column: "AccountID1",
                principalTable: "Accounts",
                principalColumn: "AccountID");
        }
    }
}
