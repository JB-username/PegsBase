using PegsBase.Models.QuickCalcs;

namespace PegsBase.Services.QuickCalcs.Interfaces
{
    public interface ICoordinateConversionService
    {
        Task<CoordinateConversionResult> ConvertAsync(
            double x, double y, int sourceSrid, int targetSrid);
    }
}
