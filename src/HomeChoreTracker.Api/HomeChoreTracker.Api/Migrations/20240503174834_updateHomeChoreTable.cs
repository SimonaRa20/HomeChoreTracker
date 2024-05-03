using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class updateHomeChoreTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "HomeChoreTasks",
                newName: "MinutesTime");

            migrationBuilder.RenameColumn(
                name: "Time",
                table: "HomeChoresBases",
                newName: "MinutesTime");

            migrationBuilder.AddColumn<int>(
                name: "HoursTime",
                table: "HomeChoreTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoursTime",
                table: "HomeChoresBases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursTime",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "HoursTime",
                table: "HomeChoresBases");

            migrationBuilder.RenameColumn(
                name: "MinutesTime",
                table: "HomeChoreTasks",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "MinutesTime",
                table: "HomeChoresBases",
                newName: "Time");
        }
    }
}
