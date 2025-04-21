using PegsBase.Models;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface IMapImportModelsToPegs
    {
       Task<List<PegRegister>> MapAsync(List<PegRegisterImportModel> importModels);
    }
}
