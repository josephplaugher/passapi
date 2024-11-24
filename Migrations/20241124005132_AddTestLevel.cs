using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passapi.Migrations
{
    /// <inheritdoc />
    public partial class AddTestLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestLevel",
                table: "TestResults",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestLevel",
                table: "TestResults");
        }
    }
}
