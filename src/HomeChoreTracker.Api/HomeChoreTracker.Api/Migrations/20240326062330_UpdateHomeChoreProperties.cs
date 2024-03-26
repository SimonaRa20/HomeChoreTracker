using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomeChoreProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Points",
                table: "TaskAssignments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "TaskAssignments");
        }
    }
}
