using PegsBase.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

public class PegPreviewReportDocument : IDocument
{
    private readonly PegPreviewModel _model;

    public PegPreviewReportDocument(PegPreviewModel model)
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
              page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Open Sans").Light());

              page.Header().Element(ComposeHeader);

              page.Content()
                  .Background(Colors.White)
                  .Border(1)
                  .BorderColor(Colors.Grey.Lighten2)
                  .Padding(10)
                  .Element(ComposeBody);

              page.Footer().AlignCenter().Text(txt =>
              {
                  txt.Span("Generated with PegsBase");
              });
          });
    }


    void ComposeHeader(IContainer header)
    {

        // 1) Style the header container itself
        header
          .Background(Colors.Blue.Medium)   // bootstrap primary
          .Padding(8)
          .Element(c => c.Row(row =>
            {
              row.RelativeItem().Column(col =>
                  {
                          col.Item().Text($"Survey Point: {_model.PegName}")
                             .FontSize(16).Light().FontColor(Colors.White);

                          col.Item().Text($"{_model.SurveyDate:yyyy/MM/dd}")
                             .FontSize(12).FontColor(Colors.White);
                      });

              row.ConstantItem(80)
                     .AlignRight()
                     .Text(_model.Type?.ToString() ?? "")
                     .FontSize(12)
                     .SemiBold();
            }));
    }

    void ComposeBody(IContainer body)
    {
        body.Column(col =>
        {
            // 1) Coordinates table
            col.Item().Table(table =>
            {
                // Define 3 equal columns
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                // 1) “Coordinates” title, spanning all 3 columns
                table.Cell()
                     .AlignLeft()
                     .Text("Coordinates:")
                     .FontSize(12)
                     .SemiBold();

                // 2) Header row
                table.Cell().Text("Y Coord").AlignCenter();
                table.Cell().Text("X Coord").AlignCenter();
                table.Cell().Text("Z Coord").AlignCenter();

                table.Cell().ColumnSpan(4).Text("");

                // 3) Data row
                table.Cell().Text("").AlignLeft();
                table.Cell().Text(_model.YCoord.ToString("F3")).AlignCenter();
                table.Cell().Text(_model.XCoord.ToString("F3")).AlignCenter();
                table.Cell().Text(_model.ZCoord.ToString("F3")).AlignCenter();

                table.Cell().ColumnSpan(4).Text("");

                // 4) Grade Elevation row
                table.Cell()
                     .AlignLeft()
                     .Text("Grade Elevation:")
                     .FontSize(12)
                     .SemiBold();
                table.Cell().Text("");
                table.Cell().Text("");
                table.Cell().Text(_model.GradeElevation?.ToString("F3")).AlignCenter();

                table.Cell().ColumnSpan(4).Text("");
            });

            // 2) Divider line with padding
            col.Item()
               .PaddingVertical(5)
               .LineHorizontal(1)
               .LineColor(Colors.Grey.Lighten1);

            // 3) Metadata row (Level, Locality, Surveyed By)
            col.Item().Element(c => c.Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"Level: {_model.Level?.Name ?? "—"}");
                    c.Item().Text($"Locality: {_model.Locality?.Name ?? "—"}");
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Surveyed By:");
                    var name = _model.Surveyor != null
                        ? $"{_model.Surveyor.FirstName} {_model.Surveyor.LastName}"
                        : _model.SurveyorId ?? "—";
                    c.Item().Text(name);
                });
            }));

            // 4) Pass/Fail row
            col.Item()
               .Background(_model.SaveToDatabase
                      ? Colors.Red.Lighten4
                      : Colors.Green.Lighten4)
               .Border(1)
               .BorderColor(_model.SaveToDatabase
                      ? Colors.Red.Darken2
                      : Colors.Green.Darken2)
               .Padding(6)
               .AlignCenter()
               .Text(_model.SaveToDatabase ? "Failed" : "Passed")
               .FontColor(_model.SaveToDatabase
                      ? Colors.Red.Darken2
                      : Colors.Green.Darken2)
               .SemiBold();
        });
    }
}
