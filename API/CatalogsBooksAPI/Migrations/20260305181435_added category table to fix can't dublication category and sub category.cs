using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedcategorytabletofixcantdublicationcategoryandsubcategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedCategories_Books_Category_SubCategory",
                table: "UserPreferedCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferedCategories",
                table: "UserPreferedCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedCategories_Category_SubCategory",
                table: "UserPreferedCategories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Books_Category_SubCategory",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "UserPreferedCategories");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "UserPreferedCategories");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "SubCategory",
                table: "Books");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "UserPreferedCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferedCategories",
                table: "UserPreferedCategories",
                columns: new[] { "AccountID", "CategoryID" });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sbucategory = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_CategoryID",
                table: "UserPreferedCategories",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedCategories_Categories_CategoryID",
                table: "UserPreferedCategories",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedCategories_Categories_CategoryID",
                table: "UserPreferedCategories");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferedCategories",
                table: "UserPreferedCategories");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedCategories_CategoryID",
                table: "UserPreferedCategories");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryID",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "UserPreferedCategories");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "UserPreferedCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "UserPreferedCategories",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Books",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubCategory",
                table: "Books",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferedCategories",
                table: "UserPreferedCategories",
                columns: new[] { "AccountID", "Category", "SubCategory" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Books_Category_SubCategory",
                table: "Books",
                columns: new[] { "Category", "SubCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_Category_SubCategory",
                table: "UserPreferedCategories",
                columns: new[] { "Category", "SubCategory" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedCategories_Books_Category_SubCategory",
                table: "UserPreferedCategories",
                columns: new[] { "Category", "SubCategory" },
                principalTable: "Books",
                principalColumns: new[] { "Category", "SubCategory" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
