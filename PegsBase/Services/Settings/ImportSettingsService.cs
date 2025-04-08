using PegsBase.Models.Settings;

namespace PegsBase.Services.Settings
{
    public class ImportSettingsService : IImportSettingsService
    {
        private ImportSettings _settings = new ImportSettings(); // You could load this from a file or DB

        public ImportSettings GetSettings() => _settings;

        public void SaveSettings(ImportSettings settings)
        {
            _settings = settings;
            // Save to DB or config file if needed
        }
    }
}
