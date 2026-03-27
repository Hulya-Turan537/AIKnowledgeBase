using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIKnowledgeBase.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDocumentFileNameTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilName",
                table: "Documents",
                newName: "FileName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Documents",
                newName: "FilName");
        }
    }
}
