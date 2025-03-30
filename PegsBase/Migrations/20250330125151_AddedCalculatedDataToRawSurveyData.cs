using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PegsBase.Migrations
{
    /// <inheritdoc />
    public partial class AddedCalculatedDataToRawSurveyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BackBearingReturn",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BackCheckHorizontalDifference",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BackCheckHorizontalDistance",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BackCheckPegElevations",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BackCheckVerticalError",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BacksightPegX",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BacksightPegY",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BacksightPegZ",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeltaX",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeltaY",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DeltaZ",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ForwardBearing",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ForwardBearingReturn",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleDirectReducedArc1",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleDirectReducedArc2",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleMeanArc1",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleMeanArc2",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleMeanFinal",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleMeanFinalReturn",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleTransitReducedArc1",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HAngleTransitReducedArc2",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorizontalDistanceBacksight",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "HorizontalDistanceForesight",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewPegX",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewPegY",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewPegZ",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StationPegX",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StationPegY",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "StationPegZ",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleBacksightMeanArc1",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleBacksightMeanArc2",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleBacksightMeanFinal",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleForesightMeanArc1",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleForesightMeanArc2",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAngleForesightMeanFinal",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VerticalDifferenceBacksight",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VerticalDifferenceForesight",
                table: "RawSurveyData",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackBearingReturn",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BackCheckHorizontalDifference",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BackCheckHorizontalDistance",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BackCheckPegElevations",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BackCheckVerticalError",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BacksightPegX",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BacksightPegY",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "BacksightPegZ",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "DeltaX",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "DeltaY",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "DeltaZ",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "ForwardBearing",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "ForwardBearingReturn",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleDirectReducedArc1",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleDirectReducedArc2",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleMeanArc1",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleMeanArc2",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleMeanFinal",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleMeanFinalReturn",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleTransitReducedArc1",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HAngleTransitReducedArc2",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HorizontalDistanceBacksight",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "HorizontalDistanceForesight",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "NewPegX",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "NewPegY",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "NewPegZ",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "StationPegX",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "StationPegY",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "StationPegZ",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleBacksightMeanArc1",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleBacksightMeanArc2",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleBacksightMeanFinal",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleForesightMeanArc1",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleForesightMeanArc2",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VAngleForesightMeanFinal",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VerticalDifferenceBacksight",
                table: "RawSurveyData");

            migrationBuilder.DropColumn(
                name: "VerticalDifferenceForesight",
                table: "RawSurveyData");
        }
    }
}
