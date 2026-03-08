using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class trytogetbookseriestablebyrenamingitintoseire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "BookSeires");

            migrationBuilder.CreateTable(
                name: "Seires",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "int", nullable: false),
                    SeireName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seires", x => x.BookID);
                    table.ForeignKey(
                        name: "FK_Seires_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "Seires");

            // migrationBuilder.CreateTable(
            //     name: "BookSeires",
            //     columns: table => new
            //     {
            //         BookID = table.Column<int>(type: "int", nullable: false),
            //         SeireName = table.Column<string>(type: "nvarchar(max)", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_BookSeires", x => x.BookID);
            //         table.ForeignKey(
            //             name: "FK_BookSeires_Books_BookID",
            //             column: x => x.BookID,
            //             principalTable: "Books",
            //             principalColumn: "BookID",
            //             onDelete: ReferentialAction.Cascade);
            //     });
        }
    }
}
