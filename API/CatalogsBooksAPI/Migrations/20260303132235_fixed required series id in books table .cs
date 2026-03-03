using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class fixedrequiredseriesidinbookstable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookSeires_SeireID",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "SeireID",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookSeires_SeireID",
                table: "Books",
                column: "SeireID",
                principalTable: "BookSeires",
                principalColumn: "SeireID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookSeires_SeireID",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "SeireID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookSeires_SeireID",
                table: "Books",
                column: "SeireID",
                principalTable: "BookSeires",
                principalColumn: "SeireID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
