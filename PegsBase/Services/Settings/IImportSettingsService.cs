using PegsBase.Models.Settings;

namespace PegsBase.Services.Settings
{
    public interface IImportSettingsService
    {
        ImportSettings GetSettings();
        void SaveSettings(ImportSettings settings);
    }
}
