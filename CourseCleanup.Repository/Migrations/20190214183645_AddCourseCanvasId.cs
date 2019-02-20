using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseCleanup.Repository.Migrations
{
    public partial class AddCourseCanvasId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CourseCanvasId",
                table: "UnusedCourse",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseCanvasId",
                table: "UnusedCourse");
        }
    }
}
