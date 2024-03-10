using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTaskAssigmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "HomeChoreTasks");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TaskAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "TaskAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "IsDone",
                table: "TaskAssignments");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "HomeChoreTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDone",
                table: "HomeChoreTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
