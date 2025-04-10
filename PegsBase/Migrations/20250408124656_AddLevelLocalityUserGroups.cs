using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelLocalityUserGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "Locality",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "Locality",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "Surveyor",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "MinePlans");

            migrationBuilder.RenameColumn(
                name: "UploadedBy",
                table: "SurveyNotes",
                newName: "UploadedById");

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "SurveyNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "SurveyNotes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "PegRegister",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "PegRegister",
                type: "integer",
                maxLength: 200,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SurveyorId",
                table: "PegRegister",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "MinePlans",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocalityId",
                table: "MinePlans",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserGroupId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Localities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    LevelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Localities_Levels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyNotes_LevelId",
                table: "SurveyNotes",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyNotes_LocalityId",
                table: "SurveyNotes",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyNotes_UploadedById",
                table: "SurveyNotes",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_PegRegister_LevelId",
                table: "PegRegister",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_PegRegister_LocalityId",
                table: "PegRegister",
                column: "LocalityId");

            migrationBuilder.CreateIndex(
                name: "IX_PegRegister_SurveyorId",
                table: "PegRegister",
                column: "SurveyorId");

            migrationBuilder.CreateIndex(
                name: "IX_MinePlans_LevelId",
                table: "MinePlans",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserGroupId",
                table: "AspNetUsers",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Localities_LevelId",
                table: "Localities",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserGroups_UserGroupId",
                table: "AspNetUsers",
                column: "UserGroupId",
                principalTable: "UserGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MinePlans_Levels_LevelId",
                table: "MinePlans",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister",
                column: "SurveyorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PegRegister_Localities_LocalityId",
                table: "PegRegister",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyNotes_AspNetUsers_UploadedById",
                table: "SurveyNotes",
                column: "UploadedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyNotes_Levels_LevelId",
                table: "SurveyNotes",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SurveyNotes_Localities_LocalityId",
                table: "SurveyNotes",
                column: "LocalityId",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserGroups_UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MinePlans_Levels_LevelId",
                table: "MinePlans");

            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_AspNetUsers_SurveyorId",
                table: "PegRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_Levels_LevelId",
                table: "PegRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_PegRegister_Localities_LocalityId",
                table: "PegRegister");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyNotes_AspNetUsers_UploadedById",
                table: "SurveyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyNotes_Levels_LevelId",
                table: "SurveyNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_SurveyNotes_Localities_LocalityId",
                table: "SurveyNotes");

            migrationBuilder.DropTable(
                name: "Localities");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_SurveyNotes_LevelId",
                table: "SurveyNotes");

            migrationBuilder.DropIndex(
                name: "IX_SurveyNotes_LocalityId",
                table: "SurveyNotes");

            migrationBuilder.DropIndex(
                name: "IX_SurveyNotes_UploadedById",
                table: "SurveyNotes");

            migrationBuilder.DropIndex(
                name: "IX_PegRegister_LevelId",
                table: "PegRegister");

            migrationBuilder.DropIndex(
                name: "IX_PegRegister_LocalityId",
                table: "PegRegister");

            migrationBuilder.DropIndex(
                name: "IX_PegRegister_SurveyorId",
                table: "PegRegister");

            migrationBuilder.DropIndex(
                name: "IX_MinePlans_LevelId",
                table: "MinePlans");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "SurveyNotes");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "SurveyorId",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "MinePlans");

            migrationBuilder.DropColumn(
                name: "LocalityId",
                table: "MinePlans");

            migrationBuilder.DropColumn(
                name: "UserGroupId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "UploadedById",
                table: "SurveyNotes",
                newName: "UploadedBy");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "SurveyNotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "SurveyNotes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "PegRegister",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "PegRegister",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Surveyor",
                table: "PegRegister",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "MinePlans",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
