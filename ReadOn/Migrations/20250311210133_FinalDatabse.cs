using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadOn.Migrations
{
    /// <inheritdoc />
    public partial class FinalDatabse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ApplicationAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "ApplicationAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
