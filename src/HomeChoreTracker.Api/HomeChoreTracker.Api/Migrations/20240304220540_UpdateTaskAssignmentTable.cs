using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HomeChoreTaskId",
                table: "TaskApprovals",
                newName: "TaskAssignmentId");

            migrationBuilder.AddColumn<DateTime>(
                name: "SetDate",
                table: "TaskAssignments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "HomeChoreTasks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetDate",
                table: "TaskAssignments");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "HomeChoreTasks");

            migrationBuilder.RenameColumn(
                name: "TaskAssignmentId",
                table: "TaskApprovals",
                newName: "HomeChoreTaskId");
        }
    }
}
