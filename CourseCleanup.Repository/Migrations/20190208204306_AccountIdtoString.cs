using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseCleanup.Repository.Migrations
{
    public partial class AccountIdtoString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AccountId",
                table: "UnusedCourse",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AccountId",
                table: "UnusedCourse",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
