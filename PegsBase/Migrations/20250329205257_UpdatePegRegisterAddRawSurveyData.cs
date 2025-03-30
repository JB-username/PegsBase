using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePegRegisterAddRawSurveyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FromPeg",
                table: "PegRegister",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPegCalc",
                table: "PegRegister",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PegFailed",
                table: "PegRegister",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RawSurveyData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Surveyor = table.Column<string>(type: "text", nullable: true),
                    SurveyDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Locality = table.Column<string>(type: "text", nullable: true),
                    StationPeg = table.Column<string>(type: "text", nullable: true),
                    BackSightPeg = table.Column<string>(type: "text", nullable: true),
                    ForeSightPeg = table.Column<string>(type: "text", nullable: true),
                    InstrumentHeight = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetHeightBacksight = table.Column<decimal>(type: "numeric", nullable: false),
                    TargetHeightForesight = table.Column<decimal>(type: "numeric", nullable: false),
                    SlopeDistanceBacksight = table.Column<decimal>(type: "numeric", nullable: false),
                    SlopeDistanceForesight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleDirectArc1Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleDirectArc1Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleTransitArc1Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleTransitArc1Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleDirectArc2Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleDirectArc2Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleTransitArc2Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    HAngleTransitArc2Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleDirectArc1Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleDirectArc1Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleTransitArc1Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleTransitArc1Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleDirectArc2Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleDirectArc2Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleTransitArc2Backsight = table.Column<decimal>(type: "numeric", nullable: false),
                    VAngleTransitArc2Foresight = table.Column<decimal>(type: "numeric", nullable: false),
                    PegFailed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawSurveyData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "FromPeg",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "HasPegCalc",
                table: "PegRegister");

            migrationBuilder.DropColumn(
                name: "PegFailed",
                table: "PegRegister");
        }
    }
}
