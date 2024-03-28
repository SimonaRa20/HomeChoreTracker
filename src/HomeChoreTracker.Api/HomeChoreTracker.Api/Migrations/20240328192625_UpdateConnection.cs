using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialRecords_Users_UserId",
                table: "FinancialRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialRecords_Users_UserId",
                table: "FinancialRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialRecords_Users_UserId",
                table: "FinancialRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialRecords_Users_UserId",
                table: "FinancialRecords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
