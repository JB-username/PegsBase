using PegsBase.Models;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

public class BasicPegViewReportDocument : IDocument
{
    private readonly PegPreviewModel _model;

    public BasicPegViewReportDocument(PegPreviewModel model)
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
                             .FontSize(10).FontColor(Colors.White);
                      });

              row.ConstantItem(80)
                     .AlignRight()
                     .Text(_model.Type?.ToString() ?? "")
                     .FontSize(10)
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
                // Define 4 equal columns
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
                     .FontSize(10)
                     .SemiBold();

                // 2) Header row
                table.Cell().Text("Y Coord").AlignLeft();
                table.Cell().Text("X Coord").AlignLeft();
                table.Cell().Text("Z Coord").AlignLeft();

                table.Cell().ColumnSpan(4).Text("");

                // 3) Data row
                table.Cell().Text("").AlignLeft();
                table.Cell().Text(_model.YCoord.ToString("F3")).AlignLeft();
                table.Cell().Text(_model.XCoord.ToString("F3")).AlignLeft();
                table.Cell().Text(_model.ZCoord.ToString("F3")).AlignLeft();

                table.Cell().ColumnSpan(4).Text("");

                // 4) Grade Elevation row
                table.Cell()
                     .AlignLeft()
                     .Text("Grade Elevation:")
                     .FontSize(10)
                     .SemiBold();
                table.Cell().Text("");
                table.Cell().Text("");
                table.Cell().Text(_model.GradeElevation?.ToString("F3")).AlignLeft();

                table.Cell().ColumnSpan(4).Text("");
            });

            // 2) Divider line with padding
            col.Item()
               .PaddingVertical(5)
               .LineHorizontal(1)
               .LineColor(Colors.Grey.Lighten1);

            // 3) Metadata table
            col.Item().PaddingTop(15).Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                table.Cell()
                     .AlignLeft()
                     .Text("Level:")
                     .FontSize(10)
                     .SemiBold();
                table.Cell().AlignLeft().Text($"{_model.Level?.Name ?? "—"}");

                table.Cell()
                     .AlignLeft()
                     .Text("Locality:")
                     .FontSize(10)
                     .SemiBold();
                table.Cell().AlignLeft().Text($"{_model.Locality?.Name ?? "—"}");

                table.Cell().ColumnSpan(4).Text("");

                table.Cell()
                     .AlignLeft()
                     .Text("Surveyor:")
                     .FontSize(10)
                     .SemiBold();
                var name = _model.Surveyor != null
                    ? $"{_model.Surveyor.FirstName} {_model.Surveyor.LastName}"
                    : _model.FallBackSurveyorName;
                table.Cell().AlignLeft().Text($"{name}");

                table.Cell()
                     .AlignLeft()
                     .Text("Check:")
                     .FontSize(10)
                     .SemiBold();
                table.Cell()
                     .AlignLeft()
                     .Text("Passed")
                     .BackgroundColor(Color.FromRGB(204,255,204))
                     .FontColor(Color.FromRGB(0, 153, 75))
                     ;
            });
        });
    }
}
