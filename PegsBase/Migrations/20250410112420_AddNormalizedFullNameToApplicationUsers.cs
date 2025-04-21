using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedFullNameToApplicationUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister");

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "PegRegister",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedFullName",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "NormalizedFullName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "PegRegister",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id");
        }
    }
}
