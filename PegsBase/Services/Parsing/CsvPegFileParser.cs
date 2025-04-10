﻿using PegsBase.Models;
using PegsBase.Models.Enums;
using PegsBase.Services.Parsing.Interfaces;

namespace PegsBase.Services.Parsing
{
    public class CsvPegFileParser : IPegFileParser
    {
        public List<PegRegisterImportModel> Parse(Stream fileStream)
        {
            var pegs = new List<PegRegisterImportModel>();

            using var reader = new StreamReader(fileStream);
            string? headerLine = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(headerLine))
                throw new Exception("File is empty or missing headers.");

            var headers = headerLine.Split(',').Select(h => h.Trim().ToLower()).ToArray();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var values = line.Split(',');

                var peg = new PegRegisterImportModel();

                for (int i = 0; i < headers.Length; i++)
                {
                    string column = headers[i];
                    string value = values.ElementAtOrDefault(i)?.Trim() ?? "";

                    switch (column)
                    {
                        case "pegname":
                            peg.PegName = value;
                            break;
                        case "xcoord":
                            peg.XCoord = decimal.Parse(value);
                            break;
                        case "ycoord":
                            peg.YCoord = decimal.Parse(value);
                            break;
                        case "zcoord":
                            peg.ZCoord = decimal.Parse(value);
                            break;
                        case "gradeelevation":
                            peg.GradeElevation = decimal.Parse(value);
                            break;
                        case "surveyor":
                            peg.SurveyorName = value;
                            break;
                        case "locality":
                            peg.LocalityName = value;
                            break;
                        case "level":
                            peg.LevelName = value;
                            break;
                        case "surveydate":
                            if (DateOnly.TryParse(value, out var dateOnly))
                                peg.SurveyDate = dateOnly;
                            break;
                        case "pointtype":
                            if (Enum.TryParse<SurveyPointType>(value, true, out var type))
                                peg.PointType = type;
                            break;
                    }
                }

                pegs.Add(peg);
            }

            return pegs;
        }

    }
}
