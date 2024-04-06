using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Homes_GamificationLevelId",
                table: "Homes");

            migrationBuilder.CreateIndex(
                name: "IX_Homes_GamificationLevelId",
                table: "Homes",
                column: "GamificationLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Homes_GamificationLevelId",
                table: "Homes");

            migrationBuilder.CreateIndex(
                name: "IX_Homes_GamificationLevelId",
                table: "Homes",
                column: "GamificationLevelId",
                unique: true);
        }
    }
}
