using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedtherelationshipsandthetables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PermissionLevel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "BookSeires",
                columns: table => new
                {
                    SeireID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeireName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookSeires", x => x.SeireID);
                });

            migrationBuilder.CreateTable(
                name: "UserLists",
                columns: table => new
                {
                    ListID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    ListName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLists", x => x.ListID);
                    table.ForeignKey(
                        name: "FK_UserLists_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AuthorID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SeireID = table.Column<int>(type: "int", nullable: false),
                    PublicationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CanDownload = table.Column<bool>(type: "bit", nullable: false),
                    DownloadLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CoverImageLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverAlt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagesCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.BookID);
                    table.UniqueConstraint("AK_Books_AuthorName", x => x.AuthorName);
                    table.UniqueConstraint("AK_Books_Category_SubCategory", x => new { x.Category, x.SubCategory });
                    table.ForeignKey(
                        name: "FK_Books_Accounts_AuthorID",
                        column: x => x.AuthorID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID");
                    table.ForeignKey(
                        name: "FK_Books_BookSeires_SeireID",
                        column: x => x.SeireID,
                        principalTable: "BookSeires",
                        principalColumn: "SeireID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookLists",
                columns: table => new
                {
                    ListID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookLists", x => new { x.BookID, x.ListID });
                    table.ForeignKey(
                        name: "FK_BookLists_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookLists_UserLists_ListID",
                        column: x => x.ListID,
                        principalTable: "UserLists",
                        principalColumn: "ListID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RateValue = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_Reviews_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferedAuthors",
                columns: table => new
                {
                    AuthorName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferedAuthors", x => new { x.AccountID, x.AuthorName });
                    table.ForeignKey(
                        name: "FK_UserPreferedAuthors_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferedAuthors_Books_AuthorName",
                        column: x => x.AuthorName,
                        principalTable: "Books",
                        principalColumn: "AuthorName",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferedAuthors_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID");
                });

            migrationBuilder.CreateTable(
                name: "UserPreferedCategories",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferedCategories", x => new { x.AccountID, x.Category, x.SubCategory });
                    table.ForeignKey(
                        name: "FK_UserPreferedCategories_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferedCategories_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID");
                    table.ForeignKey(
                        name: "FK_UserPreferedCategories_Books_Category_SubCategory",
                        columns: x => new { x.Category, x.SubCategory },
                        principalTable: "Books",
                        principalColumns: new[] { "Category", "SubCategory" },
                        onDelete: ReferentialAction.Cascade);
                });

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
                    table.ForeignKey(
                        name: "FK_ViewedBooks_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViewedBooks_Books_BookID",
                        column: x => x.BookID,
                        principalTable: "Books",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookLists_ListID",
                table: "BookLists",
                column: "ListID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorID",
                table: "Books",
                column: "AuthorID");

            migrationBuilder.CreateIndex(
                name: "IX_Books_SeireID",
                table: "Books",
                column: "SeireID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_AccountID",
                table: "Reviews",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookID",
                table: "Reviews",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLists_AccountID",
                table: "UserLists",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedAuthors_AuthorName",
                table: "UserPreferedAuthors",
                column: "AuthorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedAuthors_BookID",
                table: "UserPreferedAuthors",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_BookID",
                table: "UserPreferedCategories",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferedCategories_Category_SubCategory",
                table: "UserPreferedCategories",
                columns: new[] { "Category", "SubCategory" });

            migrationBuilder.CreateIndex(
                name: "IX_ViewedBooks_BookID",
                table: "ViewedBooks",
                column: "BookID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookLists");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "UserPreferedAuthors");

            migrationBuilder.DropTable(
                name: "UserPreferedCategories");

            migrationBuilder.DropTable(
                name: "ViewedBooks");

            migrationBuilder.DropTable(
                name: "UserLists");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "BookSeires");
        }
    }
}
