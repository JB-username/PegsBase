using PegsBase.Models;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface ICoordinateDatParserService
    {
        Task<List<PegPreviewModel>> ParseDatAsync(StreamReader reader);
    }
}
