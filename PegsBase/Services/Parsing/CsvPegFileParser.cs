using PegsBase.Models;
using PegsBase.Models.Enums;
using PegsBase.Services.Parsing.Interfaces;

namespace PegsBase.Services.Parsing
{
    public class CsvPegFileParser : IPegFileParser
    {
        public List<CsvParseResult> Parse(Stream fileStream)
        {
            var results = new List<CsvParseResult>();
            using var reader = new StreamReader(fileStream);

            // Read and validate header
            var headerLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(headerLine))
                throw new Exception("Empty or missing headers.");

            var headers = headerLine
                .Split(',')
                .Select(h => h.Trim().ToLower())
                .ToArray();

            int rowNum = 1;
            while (!reader.EndOfStream)
            {
                rowNum++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');
                var result = new CsvParseResult { RowNumber = rowNum };
                var peg = result.Peg;

                for (int i = 0; i < headers.Length; i++)
                {
                    var col = headers[i];
                    var raw = values.ElementAtOrDefault(i)?.Trim() ?? "";

                    switch (col)
                    {
                        case "pegname":
                            peg.PegName = raw;
                            if (string.IsNullOrWhiteSpace(raw))
                                result.Errors.Add("Missing PegName");
                            break;
                        case "xcoord":
                            if (decimal.TryParse(raw, out var x)) peg.XCoord = x;
                            else result.Errors.Add($"XCoord '{raw}' invalid");
                            break;
                        case "ycoord":
                            if (decimal.TryParse(raw, out var y)) peg.YCoord = y;
                            else result.Errors.Add($"YCoord '{raw}' invalid");
                            break;
                        case "zcoord":
                            if (decimal.TryParse(raw, out var z)) peg.ZCoord = z;
                            else result.Errors.Add($"ZCoord '{raw}' invalid");
                            break;
                        case "gradeelevation":
                            if (decimal.TryParse(raw, out var g)) peg.GradeElevation = g;
                            else if (!string.IsNullOrEmpty(raw))
                                result.Errors.Add($"GradeElevation '{raw}' invalid");
                            break;
                        case "surveyor":
                            peg.SurveyorName = raw;
                            break;
                        case "locality":
                            peg.LocalityName = raw;
                            break;
                        case "level":
                            peg.LevelName = raw;
                            break;
                        case "surveydate":
                            if (DateOnly.TryParse(raw, out var dt)) peg.SurveyDate = dt;
                            else if (!string.IsNullOrEmpty(raw))
                                result.Errors.Add($"SurveyDate '{raw}' invalid");
                            break;
                        case "pointtype":
                            if (Enum.TryParse<SurveyPointType>(raw, true, out var pt))
                                peg.PointType = pt;
                            else result.Errors.Add($"PointType '{raw}' invalid");
                            break;
                    }
                }

                results.Add(result);
            }

            return results;
        }
    }
}
