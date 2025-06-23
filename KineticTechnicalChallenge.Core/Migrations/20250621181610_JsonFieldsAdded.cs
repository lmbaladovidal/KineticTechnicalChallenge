using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KineticTechnicalChallenge.Core.Migrations
{
    /// <inheritdoc />
    public partial class JsonFieldsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilesJson",
                table: "Processes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilesJson",
                table: "Processes");
        }
    }
}
