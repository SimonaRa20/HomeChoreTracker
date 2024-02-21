using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFinanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionDuration",
                table: "Incomes");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionDuration",
                table: "Expenses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubscriptionDuration",
                table: "Expenses");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionDuration",
                table: "Incomes",
                type: "int",
                nullable: true);
        }
    }
}
