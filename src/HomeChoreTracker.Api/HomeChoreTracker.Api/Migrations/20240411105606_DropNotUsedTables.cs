using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class DropNotUsedTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Schedules_ScheduleId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskAssignments_Schedules_ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "TaskApprovals");

            migrationBuilder.DropTable(
                name: "TaskSchedules");

            migrationBuilder.DropIndex(
                name: "IX_TaskAssignments_ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Events_ScheduleId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "TaskAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Events",
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
                name: "TaskApprovals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HomeMemberId = table.Column<int>(type: "int", nullable: false),
                    TaskAssignmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskApprovals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaskSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskSchedules", x => x.Id);
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
                name: "FK_Events_Schedules_ScheduleId",
                table: "Events",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAssignments_Schedules_ScheduleId",
                table: "TaskAssignments",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }
    }
}
