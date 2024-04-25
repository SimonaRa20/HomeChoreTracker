using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePointHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HomeId",
                table: "PointsHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "HomeId",
                table: "PointsHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
