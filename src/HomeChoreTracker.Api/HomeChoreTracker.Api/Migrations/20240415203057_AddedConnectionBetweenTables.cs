using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedConnectionBetweenTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PointsHistory_HomeMemberId",
                table: "PointsHistory",
                column: "HomeMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Advices_UserId",
                table: "Advices",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advices_Users_UserId",
                table: "Advices",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PointsHistory_Users_HomeMemberId",
                table: "PointsHistory",
                column: "HomeMemberId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advices_Users_UserId",
                table: "Advices");

            migrationBuilder.DropForeignKey(
                name: "FK_PointsHistory_Users_HomeMemberId",
                table: "PointsHistory");

            migrationBuilder.DropIndex(
                name: "IX_PointsHistory_HomeMemberId",
                table: "PointsHistory");

            migrationBuilder.DropIndex(
                name: "IX_Advices_UserId",
                table: "Advices");
        }
    }
}
