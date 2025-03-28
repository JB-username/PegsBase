using PegsBase.Models;
using PegsBase.Models.ViewModels;

namespace PegsBase.Services.Parsing.Interfaces
{
    public interface IRawSurveyDataDatFileParser
    {
        Task<List<PegCalcViewModel>> ParseRawSurveyFileAsync(IFormFile file);
    }
}
