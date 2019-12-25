using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Quizzes.Migrations
{
    public partial class AddTimeTest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TestTime",
                table: "UrlTestAttends",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestTime",
                table: "UrlTestAttends");
        }
    }
}
