using Microsoft.EntityFrameworkCore.Migrations;

namespace Quizzes.Migrations
{
    public partial class EditUrlTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfRuns",
                table: "UrlTests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRuns",
                table: "UrlTests");
        }
    }
}
