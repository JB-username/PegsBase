using PegsBase.Models.QuickCalcs;

namespace PegsBase.Services.QuickCalcs.Interfaces
{
    public interface IJoinCalculatorService
    {
        JoinCalculatorResult Calculate(
            decimal x1, decimal y1, decimal z1,
            decimal x2, decimal y2, decimal z2);
    }
}
