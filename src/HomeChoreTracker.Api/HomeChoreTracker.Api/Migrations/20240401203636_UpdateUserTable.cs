using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "Evening",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MiddleDay",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Morning",
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

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndDayTime",
                table: "Users",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartDayTime",
                table: "Users",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateTable(
                name: "BusyIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusyIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusyIntervals_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusyIntervals_UserId",
                table: "BusyIntervals",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusyIntervals");

            migrationBuilder.DropColumn(
                name: "EndDayTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StartDayTime",
                table: "Users");

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

            migrationBuilder.AddColumn<bool>(
                name: "Evening",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MiddleDay",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Morning",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
    }
}
