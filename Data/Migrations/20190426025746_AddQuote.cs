using Microsoft.EntityFrameworkCore.Migrations;

namespace HW_06.Data.Migrations
{
    public partial class AddQuote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Quote",
                table: "Pages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quote",
                table: "Pages");
        }
    }
}
