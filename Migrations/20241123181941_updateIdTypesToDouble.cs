using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace passapi.Migrations
{
    /// <inheritdoc />
    public partial class updateIdTypesToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TestId",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<double>(
                name: "StudentId",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "TestResults",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "CustomerId",
                table: "TestResults",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "TestId",
                table: "TestResults",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "TestResults",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "TestResults",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "TestResults",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
