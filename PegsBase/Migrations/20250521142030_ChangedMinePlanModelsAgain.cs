using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMinePlanModelsAgain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locality",
                table: "MinePlans");

            migrationBuilder.CreateIndex(
                name: "IX_MinePlans_LocalityId",
                table: "MinePlans",
                column: "LocalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_MinePlans_Localities_LocalityId",
                table: "MinePlans",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinePlans_Localities_LocalityId",
                table: "MinePlans");

            migrationBuilder.DropIndex(
                name: "IX_MinePlans_LocalityId",
                table: "MinePlans");

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "MinePlans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
