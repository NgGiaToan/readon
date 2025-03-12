using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOn.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanPreviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanPreview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanPreview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanPreview_ApplicationAccount_ApplicationAccountId",
                        column: x => x.ApplicationAccountId,
                        principalTable: "ApplicationAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LoanPreview_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoanPreview_ApplicationAccountId",
                table: "LoanPreview",
                column: "ApplicationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanPreview_BookId",
                table: "LoanPreview",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanPreview");
        }
    }
}
