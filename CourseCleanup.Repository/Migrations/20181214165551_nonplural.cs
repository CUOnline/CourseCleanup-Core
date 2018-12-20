using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseCleanup.Repository.Migrations
{
    public partial class nonplural : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnusedCourses_CourseSearchQueues_CourseSearchQueueId",
                table: "UnusedCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnusedCourses",
                table: "UnusedCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSearchQueues",
                table: "CourseSearchQueues");

            migrationBuilder.DropColumn(
                name: "EndTermId",
                table: "CourseSearchQueues");

            migrationBuilder.RenameTable(
                name: "UnusedCourses",
                newName: "UnusedCourse");

            migrationBuilder.RenameTable(
                name: "CourseSearchQueues",
                newName: "CourseSearchQueue");

            migrationBuilder.RenameIndex(
                name: "IX_UnusedCourses_CourseSearchQueueId",
                table: "UnusedCourse",
                newName: "IX_UnusedCourse_CourseSearchQueueId");

            migrationBuilder.RenameColumn(
                name: "StartTermId",
                table: "CourseSearchQueue",
                newName: "TermList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnusedCourse",
                table: "UnusedCourse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSearchQueue",
                table: "CourseSearchQueue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnusedCourse_CourseSearchQueue_CourseSearchQueueId",
                table: "UnusedCourse",
                column: "CourseSearchQueueId",
                principalTable: "CourseSearchQueue",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UnusedCourse_CourseSearchQueue_CourseSearchQueueId",
                table: "UnusedCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UnusedCourse",
                table: "UnusedCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSearchQueue",
                table: "CourseSearchQueue");

            migrationBuilder.RenameTable(
                name: "UnusedCourse",
                newName: "UnusedCourses");

            migrationBuilder.RenameTable(
                name: "CourseSearchQueue",
                newName: "CourseSearchQueues");

            migrationBuilder.RenameIndex(
                name: "IX_UnusedCourse_CourseSearchQueueId",
                table: "UnusedCourses",
                newName: "IX_UnusedCourses_CourseSearchQueueId");

            migrationBuilder.RenameColumn(
                name: "TermList",
                table: "CourseSearchQueues",
                newName: "StartTermId");

            migrationBuilder.AddColumn<string>(
                name: "EndTermId",
                table: "CourseSearchQueues",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UnusedCourses",
                table: "UnusedCourses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSearchQueues",
                table: "CourseSearchQueues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UnusedCourses_CourseSearchQueues_CourseSearchQueueId",
                table: "UnusedCourses",
                column: "CourseSearchQueueId",
                principalTable: "CourseSearchQueues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
