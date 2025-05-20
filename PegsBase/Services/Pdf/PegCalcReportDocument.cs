using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using QuestPDF.Drawing;
using PegsBase.Models.ViewModels;

public class PegCalcReportDocument : IDocument
{
    private readonly PegCalcViewModel _model;

    public PegCalcReportDocument(PegCalcViewModel model)
    {
        _model = model;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Element(ComposeHeader);
                page.Content().PaddingVertical(10).Element(ComposeBody);
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Generated with PegsBase");
                });
            });
    }

    void ComposeHeader(IContainer header)
    {
        header.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"Surveyor: {_model.SurveyorDisplayName}");
                col.Item().Text($"Locality: {_model.LocalityName}");
                col.Item().Text($"Level: {_model.LevelName}");
            });

            row.ConstantItem(100).AlignRight().Text(_model.ForeSightPeg)
                .FontSize(18).SemiBold().FontColor(Colors.Blue.Medium);
        });
    }

    void ComposeBody(IContainer body)
    {
        body.Column(col =>
        {
            col.Spacing(10);

            // Horizontal Angles Table
            col.Item().Element(ComposeHorizontalAngles);

            // Observations
            col.Item().Element(ComposeObservations);

            // Vertical Angles & Back-Check could be additional elements here...
        });
    }

    void ComposeHorizontalAngles(IContainer container)
    {
        container.Table(table =>
        {
            // define 5 columns
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(80);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            // Header row
            table.Header(header =>
            {
                header.Cell().Text("Type").Bold();
                header.Cell().Text("Dir Arc1").Bold();
                header.Cell().Text("Trn Arc1").Bold();
                header.Cell().Text("Dir Arc2").Bold();
                header.Cell().Text("Trn Arc2").Bold();
            });

            // Backsight
            table.Cell().Text($"Backsight ({_model.BackSightPeg})").SemiBold();
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectArc1Backsight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitArc1Backsight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectArc2Backsight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitArc2Backsight));

            // Foresight
            table.Cell().Text($"Foresight ({_model.ForeSightPeg})").SemiBold();
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectArc1Foresight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitArc1Foresight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectArc2Foresight));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitArc2Foresight));

            // Reduced
            table.Cell().Text("Reduced").SemiBold();
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectReducedArc1));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitReducedArc1));
            table.Cell().Text(_model.FormatDMS(_model.HAngleDirectReducedArc2));
            table.Cell().Text(_model.FormatDMS(_model.HAngleTransitReducedArc2));

            // Mean
            table.Cell().Text("Mean").SemiBold();
            table.Cell().Text(string.Empty);
            table.Cell().Text(_model.FormatDMS(_model.HAngleMeanArc1));
            table.Cell().Text(string.Empty);
            table.Cell().Text(_model.FormatDMS(_model.HAngleMeanArc2));
        });
    }

    void ComposeObservations(IContainer container)
    {
        container.Column(col =>
        {
            col.Spacing(5);

            col.Item().Text("Observations").Bold();

            // Horizontal Mean & Bearings
            col.Item().Row(row =>
            {
                row.ConstantItem(120).Text("Horizonal Mean");
                row.RelativeItem().Text(_model.FormatDMS(_model.HAngleMeanFinal));
                row.RelativeItem().Text(_model.FormatDMS(_model.HAngleMeanFinalReturn));
            });

            col.Item().Row(row =>
            {
                row.ConstantItem(120).Text("Forward Bearing");
                row.RelativeItem().Text(_model.FormatDMS(_model.ForwardBearing));
                row.RelativeItem().Text(_model.FormatDMS(_model.ForwardBearingReturn));
            });

            // you can add more rows here...
        });
    }
}

