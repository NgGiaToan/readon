using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOn.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountIdtoBooktbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Branch",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Book",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Book_AccountId",
                table: "Book",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Account_AccountId",
                table: "Book",
                column: "AccountId",
                principalTable: "Account",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Account_AccountId",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_AccountId",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Book");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "Branch",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
