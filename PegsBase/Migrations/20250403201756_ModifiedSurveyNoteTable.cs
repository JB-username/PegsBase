using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedSurveyNoteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbandonmentReason",
                table: "SurveyNotes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAbandoned",
                table: "SurveyNotes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NoteType",
                table: "SurveyNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AbandonmentReason",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "IsAbandoned",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "NoteType",
                table: "SurveyNotes");
        }
    }
}
