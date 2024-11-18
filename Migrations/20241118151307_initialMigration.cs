using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passapi.Migrations
{
    /// <inheritdoc />
    public partial class initialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    TestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<int>(type: "integer", nullable: false),
                    RawScore = table.Column<int>(type: "integer", nullable: false),
                    PercentCorrect = table.Column<int>(type: "integer", nullable: false),
                    RitScore = table.Column<int>(type: "integer", nullable: false),
                    HewittPercentile = table.Column<int>(type: "integer", nullable: false),
                    NationalPercentile = table.Column<int>(type: "integer", nullable: false),
                    OverallRank = table.Column<int>(type: "integer", nullable: false),
                    FirstGoalRank = table.Column<int>(type: "integer", nullable: false),
                    SecondGoalRank = table.Column<int>(type: "integer", nullable: false),
                    ThirdGoalRank = table.Column<int>(type: "integer", nullable: false),
                    FourthGoalRank = table.Column<int>(type: "integer", nullable: false),
                    FifthGoalRank = table.Column<int>(type: "integer", nullable: false),
                    SixthGoalRank = table.Column<int>(type: "integer", nullable: false),
                    SeventhGoalRank = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: false),
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
