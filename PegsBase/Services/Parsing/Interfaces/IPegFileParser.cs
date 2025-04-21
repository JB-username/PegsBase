using PegsBase.Models;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface IPegFileParser
    {
        List<PegRegisterImportModel> Parse(Stream fileStream);
    }
}
