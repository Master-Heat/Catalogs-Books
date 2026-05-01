using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogsBooksAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixSeriesSpelling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Rename the table from Seires to Series
            migrationBuilder.RenameTable(
                name: "Seires",
                newName: "Series");

            // 2. Rename the column from SeireName to SeriesName
            migrationBuilder.RenameColumn(
                name: "SeireName",
                table: "Series",
                newName: "SeriesName");

            // 3. Optional: Rename the Primary Key and Foreign Key constraints 
            // to keep the naming convention consistent
            migrationBuilder.RenameIndex(
                name: "PK_Seires",
                table: "Series",
                newName: "PK_Series");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the names if you need to roll back
            migrationBuilder.RenameTable(
                name: "Series",
                newName: "Seires");

            migrationBuilder.RenameColumn(
                name: "SeriesName",
                table: "Seires",
                newName: "SeireName");

            migrationBuilder.RenameIndex(
                name: "PK_Series",
                table: "Seires",
                newName: "PK_Seires");
        }
    }
}
