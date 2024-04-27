using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChallengeTableProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Challenges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpponentCount",
                table: "Challenges",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Challenges");

            migrationBuilder.DropColumn(
                name: "OpponentCount",
                table: "Challenges");
        }
    }
}
