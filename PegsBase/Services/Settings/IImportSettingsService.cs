using PegsBase.Models;

namespace PegsBase.Services.Settings
{
    public interface IImportSettingsService
    {
        AppSettings GetSettings();
        void SaveSettings(AppSettings settings);
    }
}
