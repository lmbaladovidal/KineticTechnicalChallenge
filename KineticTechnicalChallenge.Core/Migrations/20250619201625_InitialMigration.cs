using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KineticTechnicalChallenge.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstimatedCompletion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TotalFiles = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcessedFiles = table.Column<int>(type: "INTEGER", nullable: false),
                    Percentage = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessInfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TotalWords = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalLines = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalCharacters = table.Column<int>(type: "INTEGER", nullable: false),
                    MostFrequentWordsJson = table.Column<string>(type: "TEXT", nullable: false),
                    FilesProcessedJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_Processes_ProcessInfoId",
                        column: x => x.ProcessInfoId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_ProcessInfoId",
                table: "Results",
                column: "ProcessInfoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Processes");
        }
    }
}
