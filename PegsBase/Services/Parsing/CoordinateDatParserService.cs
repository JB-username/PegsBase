using PegsBase.Services.Parsing.Interfaces;
using PegsBase.Models;

namespace PegsBase.Services.Parsing
{
    public class CoordinateDatParserService : ICoordinateDatParserService
    {
        public async Task<List<PegPreviewModel>> ParseDatAsync(StreamReader reader)
        {
            var pegs = new List<PegPreviewModel>();

            string? line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 4) continue;

                try
                {
                    var peg = new PegPreviewModel
                    {
                        PegName = parts[0],
                        YCoord = decimal.Parse(parts[1]),
                        XCoord = decimal.Parse(parts[2]),
                        ZCoord = decimal.Parse(parts[3]),
                        GradeElevation = parts.Length >= 5 ? decimal.Parse(parts[4]) : null
                    };

                    pegs.Add(peg);
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            return pegs;
        }
    }
}
