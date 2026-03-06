using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class Fixedcategoryandauthoerrelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Accounts_AuthorID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedAuthors_Books_AuthorName",
                table: "UserPreferedAuthors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferedAuthors",
                table: "UserPreferedAuthors");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedAuthors_AuthorName",
                table: "UserPreferedAuthors");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Books_AuthorName",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "AuthorName",
                table: "UserPreferedAuthors");

            migrationBuilder.AddColumn<int>(
                name: "AccountID1",
                table: "UserPreferedCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorID",
                table: "UserPreferedAuthors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferedAuthors",
                table: "UserPreferedAuthors",
                columns: new[] { "AuthorID", "AccountID" });

            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    AuthorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.AuthorID);
                    table.ForeignKey(
                        name: "FK_Author_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_AccountID1",
                table: "UserPreferedCategories",
                column: "AccountID1");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedAuthors_AccountID",
                table: "UserPreferedAuthors",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Author_AccountID",
                table: "Author",
                column: "AccountID",
                unique: true,
                filter: "[AccountID] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Author_AuthorID",
                table: "Books",
                column: "AuthorID",
                principalTable: "Author",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedAuthors_Author_AuthorID",
                table: "UserPreferedAuthors",
                column: "AuthorID",
                principalTable: "Author",
                principalColumn: "AuthorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedCategories_Accounts_AccountID1",
                table: "UserPreferedCategories",
                column: "AccountID1",
                principalTable: "Accounts",
                principalColumn: "AccountID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Author_AuthorID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedAuthors_Author_AuthorID",
                table: "UserPreferedAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferedCategories_Accounts_AccountID1",
                table: "UserPreferedCategories");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedCategories_AccountID1",
                table: "UserPreferedCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferedAuthors",
                table: "UserPreferedAuthors");

            migrationBuilder.DropIndex(
                name: "IX_UserPreferedAuthors_AccountID",
                table: "UserPreferedAuthors");

            migrationBuilder.DropColumn(
                name: "AccountID1",
                table: "UserPreferedCategories");

            migrationBuilder.DropColumn(
                name: "AuthorID",
                table: "UserPreferedAuthors");

            migrationBuilder.AddColumn<string>(
                name: "AuthorName",
                table: "UserPreferedAuthors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                table: "Books",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorID",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferedAuthors",
                table: "UserPreferedAuthors",
                columns: new[] { "AccountID", "AuthorName" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Books_AuthorName",
                table: "Books",
                column: "AuthorName");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedAuthors_AuthorName",
                table: "UserPreferedAuthors",
                column: "AuthorName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Accounts_AuthorID",
                table: "Books",
                column: "AuthorID",
                principalTable: "Accounts",
                principalColumn: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferedAuthors_Books_AuthorName",
                table: "UserPreferedAuthors",
                column: "AuthorName",
                principalTable: "Books",
                principalColumn: "AuthorName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
