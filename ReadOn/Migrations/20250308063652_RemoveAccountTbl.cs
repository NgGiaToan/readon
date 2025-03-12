using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOn.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccountTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Account_AccountId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Loan_Account_AccountId",
                table: "Loan");

            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Loan",
                newName: "ApplicationAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Loan_AccountId",
                table: "Loan",
                newName: "IX_Loan_ApplicationAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Book",
                newName: "ApplicationAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Book_AccountId",
                table: "Book",
                newName: "IX_Book_ApplicationAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_ApplicationAccount_ApplicationAccountId",
                table: "Book",
                column: "ApplicationAccountId",
                principalTable: "ApplicationAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loan_ApplicationAccount_ApplicationAccountId",
                table: "Loan",
                column: "ApplicationAccountId",
                principalTable: "ApplicationAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_ApplicationAccount_ApplicationAccountId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_Loan_ApplicationAccount_ApplicationAccountId",
                table: "Loan");

            migrationBuilder.RenameColumn(
                name: "ApplicationAccountId",
                table: "Loan",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Loan_ApplicationAccountId",
                table: "Loan",
                newName: "IX_Loan_AccountId");

            migrationBuilder.RenameColumn(
                name: "ApplicationAccountId",
                table: "Book",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Book_ApplicationAccountId",
                table: "Book",
                newName: "IX_Book_AccountId");

            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Createddate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Passwordhash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Account_AccountId",
                table: "Book",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loan_Account_AccountId",
                table: "Loan",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
