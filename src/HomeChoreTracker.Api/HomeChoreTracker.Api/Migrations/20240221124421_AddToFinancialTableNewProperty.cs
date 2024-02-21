using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddToFinancialTableNewProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomeId",
                table: "Incomes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeId",
                table: "Expenses",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "HomeId",
                table: "Expenses");
        }
    }
}
