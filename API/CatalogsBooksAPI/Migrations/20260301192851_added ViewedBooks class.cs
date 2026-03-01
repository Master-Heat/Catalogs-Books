using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedViewedBooksclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ViewedBooks",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewedBooks", x => new { x.AccountID, x.BookID });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViewedBooks");
        }
    }
}
