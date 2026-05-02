using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addedmapfordeletebookasitneverexist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookID1",
                table: "ViewedBooks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViewedBooks_BookID1",
                table: "ViewedBooks",
                column: "BookID1");

            migrationBuilder.AddForeignKey(
                name: "FK_ViewedBooks_Books_BookID1",
                table: "ViewedBooks",
                column: "BookID1",
                principalTable: "Books",
                principalColumn: "BookID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ViewedBooks_Books_BookID1",
                table: "ViewedBooks");

            migrationBuilder.DropIndex(
                name: "IX_ViewedBooks_BookID1",
                table: "ViewedBooks");

            migrationBuilder.DropColumn(
                name: "BookID1",
                table: "ViewedBooks");
        }
    }
}
