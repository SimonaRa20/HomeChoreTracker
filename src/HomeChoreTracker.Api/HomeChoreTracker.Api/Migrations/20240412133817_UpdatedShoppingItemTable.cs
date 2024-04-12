using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedShoppingItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HomeChoreTaskId",
                table: "ShoppingItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Times",
                table: "ShoppingItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeChoreTaskId",
                table: "ShoppingItems");

            migrationBuilder.DropColumn(
                name: "Times",
                table: "ShoppingItems");
        }
    }
}
