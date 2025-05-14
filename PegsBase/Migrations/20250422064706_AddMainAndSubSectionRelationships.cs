using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class AddMainAndSubSectionRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubSectionId",
                table: "Localities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubSectionId",
                table: "Levels",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MainSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainSections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    MainSectionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubSections_MainSections_MainSectionId",
                        column: x => x.MainSectionId,
                        principalTable: "MainSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localities_SubSectionId",
                table: "Localities",
                column: "SubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Levels_SubSectionId",
                table: "Levels",
                column: "SubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubSections_MainSectionId",
                table: "SubSections",
                column: "MainSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_SubSections_SubSectionId",
                table: "Levels",
                column: "SubSectionId",
                principalTable: "SubSections",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Localities_SubSections_SubSectionId",
                table: "Localities",
                column: "SubSectionId",
                principalTable: "SubSections",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_SubSections_SubSectionId",
                table: "Levels");

            migrationBuilder.DropForeignKey(
                name: "FK_Localities_SubSections_SubSectionId",
                table: "Localities");

            migrationBuilder.DropTable(
                name: "SubSections");

            migrationBuilder.DropTable(
                name: "MainSections");

            migrationBuilder.DropIndex(
                name: "IX_Localities_SubSectionId",
                table: "Localities");

            migrationBuilder.DropIndex(
                name: "IX_Levels_SubSectionId",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "SubSectionId",
                table: "Localities");

            migrationBuilder.DropColumn(
                name: "SubSectionId",
                table: "Levels");
        }
    }
}
