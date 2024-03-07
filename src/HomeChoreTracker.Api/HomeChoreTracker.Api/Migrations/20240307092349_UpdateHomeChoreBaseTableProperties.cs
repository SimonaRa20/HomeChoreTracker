using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomeChoreBaseTableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "HomeChoreTasks");

            migrationBuilder.RenameColumn(
                name: "Frequency",
                table: "HomeChoresBases",
                newName: "Unit");

            migrationBuilder.AddColumn<int>(
                name: "DayOfMonth",
                table: "HomeChoresBases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "HomeChoresBases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Interval",
                table: "HomeChoresBases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MonthlyRepeatType",
                table: "HomeChoresBases",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DayOfMonth",
                table: "HomeChoresBases");

            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "HomeChoresBases");

            migrationBuilder.DropColumn(
                name: "Interval",
                table: "HomeChoresBases");

            migrationBuilder.DropColumn(
                name: "MonthlyRepeatType",
                table: "HomeChoresBases");

            migrationBuilder.RenameColumn(
                name: "Unit",
                table: "HomeChoresBases",
                newName: "Frequency");

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "HomeChoreTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
