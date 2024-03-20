using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndDayHour",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndDayMinutes",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndLunchHour",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndLunchMinutes",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartDayHour",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartDayMinutes",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartLunchHour",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StartLunchMinutes",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDayHour",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndDayMinutes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndLunchHour",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EndLunchMinutes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartDayHour",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartDayMinutes",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartLunchHour",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartLunchMinutes",
                table: "Users");
        }
    }
}
