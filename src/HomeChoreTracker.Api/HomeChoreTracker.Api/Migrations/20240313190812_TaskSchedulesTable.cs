using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class TaskSchedulesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "TaskAssignments");

            migrationBuilder.RenameColumn(
                name: "SetDate",
                table: "TaskAssignments",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "HomeChoreTaskId",
                table: "TaskAssignments",
                newName: "TaskId");

            migrationBuilder.AlterColumn<int>(
                name: "HomeMemberId",
                table: "TaskAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "TaskAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "taskSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Dates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Randomize = table.Column<bool>(type: "bit", nullable: false),
                    HomeMemberId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_taskSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignments_ScheduleId",
                table: "TaskAssignments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_ScheduleId",
                table: "Events",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Schedules_ScheduleId",
                table: "TaskAssignments",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Schedules_ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "taskSchedules");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "TaskAssignments",
                newName: "HomeChoreTaskId");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "TaskAssignments",
                newName: "SetDate");

            migrationBuilder.AlterColumn<int>(
                name: "HomeMemberId",
                table: "TaskAssignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: true);
        }
    }
}
