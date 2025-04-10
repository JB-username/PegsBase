using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class SurveyInPegRegisterNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyorId",
                table: "PegRegister",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister",
                column: "SurveyorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyorId",
                table: "PegRegister",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister",
                column: "SurveyorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
