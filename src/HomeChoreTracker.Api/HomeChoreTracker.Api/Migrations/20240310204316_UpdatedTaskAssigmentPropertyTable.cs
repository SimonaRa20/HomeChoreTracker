using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTaskAssigmentPropertyTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "TaskAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "HomeMemberId",
                table: "TaskAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
