using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseCleanup.Repository.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseSearchQueues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    StartTermId = table.Column<string>(nullable: true),
                    EndTermId = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StatusMessage = table.Column<string>(nullable: true),
                    SubmittedByEmail = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSearchQueues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnusedCourses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: true),
                    CourseId = table.Column<string>(nullable: true),
                    CourseName = table.Column<string>(nullable: true),
                    CourseSISID = table.Column<string>(nullable: true),
                    CourseCode = table.Column<string>(nullable: true),
                    TermId = table.Column<string>(nullable: true),
                    Term = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CourseSearchQueueId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnusedCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnusedCourses_CourseSearchQueues_CourseSearchQueueId",
                        column: x => x.CourseSearchQueueId,
                        principalTable: "CourseSearchQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnusedCourses_CourseSearchQueueId",
                table: "UnusedCourses",
                column: "CourseSearchQueueId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnusedCourses");

            migrationBuilder.DropTable(
                name: "CourseSearchQueues");
        }
    }
}
