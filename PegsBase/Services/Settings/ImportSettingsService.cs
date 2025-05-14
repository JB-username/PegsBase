using PegsBase.Data;
using PegsBase.Models;

namespace PegsBase.Services.Settings
{
    public class ImportSettingsService : IImportSettingsService
    {
        private readonly ApplicationDbContext _dbContext;

        public ImportSettingsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AppSettings GetSettings()
        {
            var settings = _dbContext.AppSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new AppSettings();
                _dbContext.AppSettings.Add(settings);
                _dbContext.SaveChanges();
            }

            return settings;
        }

        public void SaveSettings(AppSettings settings)
        {
            var existing = _dbContext.AppSettings.FirstOrDefault();
            if (existing != null)
            {
                existing.SwapXY = settings.SwapXY;
                existing.InvertX = settings.InvertX;
                existing.InvertY = settings.InvertY;

                _dbContext.AppSettings.Update(existing);
            }
            else
            {
                _dbContext.AppSettings.Add(settings);
            }

            _dbContext.SaveChanges();
        }
    }
}
