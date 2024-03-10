using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedHomeChoreTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "HomeChoreTasks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "HomeChoreTasks",
                newName: "Unit");

            migrationBuilder.RenameColumn(
                name: "IsAprroved",
                table: "HomeChoreTasks",
                newName: "IsApproved");

            migrationBuilder.AddColumn<int>(
                name: "DayOfMonth",
                table: "HomeChoreTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "HomeChoreTasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "HomeChoreTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelType",
                table: "HomeChoreTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyRepeatType",
                table: "HomeChoreTasks",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "LevelType",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "MonthlyRepeatType",
                table: "HomeChoreTasks");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "HomeChoreTasks",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "HomeChoreTasks",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "HomeChoreTasks",
                newName: "IsAprroved");
        }
    }
}
