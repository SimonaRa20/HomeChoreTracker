using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class updatedDatesInHomeChoresTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_taskSchedules",
                table: "taskSchedules");

            migrationBuilder.DropColumn(
                name: "Dates",
                table: "taskSchedules");

            migrationBuilder.DropColumn(
                name: "HomeMemberId",
                table: "taskSchedules");

            migrationBuilder.DropColumn(
                name: "Randomize",
                table: "taskSchedules");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "HomeChoreTasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "HomeChoreTasks");

            migrationBuilder.RenameTable(
                name: "taskSchedules",
                newName: "TaskSchedules");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskSchedules",
                table: "TaskSchedules",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskSchedules",
                table: "TaskSchedules");

            migrationBuilder.RenameTable(
                name: "TaskSchedules",
                newName: "taskSchedules");

            migrationBuilder.AddColumn<string>(
                name: "Dates",
                table: "taskSchedules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeMemberId",
                table: "taskSchedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Randomize",
                table: "taskSchedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "HomeChoreTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "HomeChoreTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_taskSchedules",
                table: "taskSchedules",
                column: "Id");
        }
    }
}
