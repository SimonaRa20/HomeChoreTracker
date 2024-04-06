using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeChoreTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHomeChoreBaseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "HomeChoresBases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BadgeWallets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoneFirstTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstCleaningTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstLaundryTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstKitchenTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstBathroomTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstBedroomTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstOutdoorsTask = table.Column<bool>(type: "bit", nullable: false),
                    DoneFirstOrganizeTask = table.Column<bool>(type: "bit", nullable: false),
                    EarnedPerDayFiftyPoints = table.Column<bool>(type: "bit", nullable: false),
                    EarnedPerDayHundredPoints = table.Column<bool>(type: "bit", nullable: false),
                    DoneFiveTaskPerWeek = table.Column<bool>(type: "bit", nullable: false),
                    DoneTenTaskPerWeek = table.Column<bool>(type: "bit", nullable: false),
                    DoneTwentyFiveTaskPerWeek = table.Column<bool>(type: "bit", nullable: false),
                    CreatedTaskWasUsedOtherHome = table.Column<bool>(type: "bit", nullable: false),
                    CreateFirstPurchase = table.Column<bool>(type: "bit", nullable: false),
                    CreateFirstAdvice = table.Column<bool>(type: "bit", nullable: false),
                    CreateFirstIncome = table.Column<bool>(type: "bit", nullable: false),
                    CreateFirstExpense = table.Column<bool>(type: "bit", nullable: false),
                    CreateFirstHomeChore = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeWallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BadgeWallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BadgeWallets_UserId",
                table: "BadgeWallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BadgeWallets");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "HomeChoresBases");
        }
    }
}
