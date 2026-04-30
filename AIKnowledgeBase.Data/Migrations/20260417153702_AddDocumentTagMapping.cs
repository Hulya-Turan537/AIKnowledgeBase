using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIKnowledgeBase.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentTagMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Documents_DocumentsId",
                table: "DocumentTag");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Tags_TagsId",
                table: "DocumentTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTag_TagsId",
                table: "DocumentTag");

            migrationBuilder.RenameColumn(
                name: "TagsId",
                table: "DocumentTag",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "DocumentsId",
                table: "DocumentTag",
                newName: "TagId");

            migrationBuilder.AddColumn<int>(
                name: "DocumentId",
                table: "DocumentTag",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "DocumentTag",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag",
                columns: new[] { "DocumentId", "TagId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_TagId",
                table: "DocumentTag",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Documents_DocumentId",
                table: "DocumentTag",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Tags_TagId",
                table: "DocumentTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Documents_DocumentId",
                table: "DocumentTag");

            migrationBuilder.DropForeignKey(
                name: "FK_DocumentTag_Tags_TagId",
                table: "DocumentTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag");

            migrationBuilder.DropIndex(
                name: "IX_DocumentTag_TagId",
                table: "DocumentTag");

            migrationBuilder.DropColumn(
                name: "DocumentId",
                table: "DocumentTag");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "DocumentTag");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "DocumentTag",
                newName: "TagsId");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "DocumentTag",
                newName: "DocumentsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DocumentTag",
                table: "DocumentTag",
                columns: new[] { "DocumentsId", "TagsId" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_TagsId",
                table: "DocumentTag",
                column: "TagsId");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Documents_DocumentsId",
                table: "DocumentTag",
                column: "DocumentsId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentTag_Tags_TagsId",
                table: "DocumentTag",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
