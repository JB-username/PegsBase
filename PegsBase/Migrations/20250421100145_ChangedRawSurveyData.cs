using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangedRawSurveyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locality",
                table: "RawSurveyData");

            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "RawSurveyData",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RawSurveyData_LocalityId",
                table: "RawSurveyData",
                column: "LocalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_RawSurveyData_Localities_LocalityId",
                table: "RawSurveyData",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RawSurveyData_Localities_LocalityId",
                table: "RawSurveyData");

            migrationBuilder.DropIndex(
                name: "IX_RawSurveyData_LocalityId",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "RawSurveyData");

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "RawSurveyData",
                type: "text",
                nullable: true);
        }
    }
}
