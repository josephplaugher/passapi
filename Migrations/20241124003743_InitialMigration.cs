using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passapi.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TestId = table.Column<int>(type: "integer", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: false),
                    RawScore = table.Column<int>(type: "integer", nullable: false),
                    PercentCorrect = table.Column<int>(type: "integer", nullable: false),
                    RitScore = table.Column<int>(type: "integer", nullable: false),
                    HewittPercentile = table.Column<int>(type: "integer", nullable: false),
                    NationalPercentile = table.Column<int>(type: "integer", nullable: false),
                    OverallRank = table.Column<string>(type: "text", nullable: false),
                    FirstGoalRank = table.Column<string>(type: "text", nullable: false),
                    SecondGoalRank = table.Column<string>(type: "text", nullable: false),
                    ThirdGoalRank = table.Column<string>(type: "text", nullable: false),
                    FourthGoalRank = table.Column<string>(type: "text", nullable: false),
                    FifthGoalRank = table.Column<string>(type: "text", nullable: false),
                    SixthGoalRank = table.Column<string>(type: "text", nullable: false),
                    SeventhGoalRank = table.Column<string>(type: "text", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    Held = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResults");
        }
    }
}
