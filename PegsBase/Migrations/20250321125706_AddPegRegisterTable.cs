using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPegRegisterTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PegRegister",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locality = table.Column<string>(type: "text", nullable: false),
                    YCoord = table.Column<decimal>(type: "numeric", nullable: false),
                    XCoord = table.Column<decimal>(type: "numeric", nullable: false),
                    ZCoord = table.Column<decimal>(type: "numeric", nullable: false),
                    GradeElevation = table.Column<decimal>(type: "numeric", nullable: false),
                    Surveyor = table.Column<string>(type: "text", nullable: false),
                    SurveyDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PegRegister", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PegRegister");
        }
    }
}
