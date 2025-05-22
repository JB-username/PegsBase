using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMinePlanTypesToDataBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PlanType",
                table: "MinePlans",
                newName: "PlanTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MinePlans_PlanTypeId",
                table: "MinePlans",
                column: "PlanTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MinePlans_PlanTypes_PlanTypeId",
                table: "MinePlans",
                column: "PlanTypeId",
                principalTable: "PlanTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MinePlans_PlanTypes_PlanTypeId",
                table: "MinePlans");

            migrationBuilder.DropIndex(
                name: "IX_MinePlans_PlanTypeId",
                table: "MinePlans");

            migrationBuilder.RenameColumn(
                name: "PlanTypeId",
                table: "MinePlans",
                newName: "PlanType");
        }
    }
}
