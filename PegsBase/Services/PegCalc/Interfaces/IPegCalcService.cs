using PegsBase.Models;
using PegsBase.Models.ViewModels;

namespace PegsBase.Services.PegCalc.Interfaces
{
    public interface IPegCalcService
    {
        PegCalcViewModel CalculatePeg(PegCalcViewModel vm, PegRegister setupPeg, PegRegister backsightPeg);
    }
}
