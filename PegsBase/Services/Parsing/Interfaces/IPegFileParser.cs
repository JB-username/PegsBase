using PegsBase.Models;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface IPegFileParser
    {
        List<CsvParseResult> Parse(Stream fileStream);
    }
}
