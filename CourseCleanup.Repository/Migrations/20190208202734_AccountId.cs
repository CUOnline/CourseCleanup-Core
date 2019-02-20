using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseCleanup.Repository.Migrations
{
    public partial class AccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "UnusedCourse",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "UnusedCourse");
        }
    }
}
