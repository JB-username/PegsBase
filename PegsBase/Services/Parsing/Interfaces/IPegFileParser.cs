using PegsBase.Models;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface IPegFileParser
    {
        List<PegRegister> Parse(Stream fileStream);
    }
}
