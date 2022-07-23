using Microsoft.EntityFrameworkCore;
using MyWebServices.Core.Models;

namespace MyWebServices.Core.DataAccess.Repositories
{
    public class UserRepository
    {
        private ParagraphsDbContext _context;

        public UserRepository(ParagraphsDbContext dbContext)
        {
            _context = dbContext;
        }

        public UserSettings GetUserSettings(int userId, int patternId)
        {
            if (patternId == 0) patternId = 1;

            var settingsEntity = _context.UsersSettings
                .Include(u => u.UserPatterns)
                .Include(u => u.SharedCustomUserElements)
                .Single(u => u.UserId == userId);

            settingsEntity.UserPatterns.ForEach(u => u.CustomUserElementsForPattern = _context.CustomUserElements.Where(el => el.UserPatternId == u.Id).ToList());

            var userSettings = new UserSettings
            {
                UserPattern = settingsEntity.UserPatterns.Single(p => p.Id == patternId),
                SharedCustomUserElements = settingsEntity.SharedCustomUserElements,
                CutElement = settingsEntity.CutElement,
                TextLengthBeforeCut = settingsEntity.TextLengthBeforeCut,
                ParagraphElement = settingsEntity.ParagraphElement,
                ParagraphCenterAlignClass = settingsEntity.ParagraphCenterAlignClass,
                ListElement = settingsEntity.ListElement,
            };

            return userSettings;
        }

        public object GetUserSettingsForView(int userId)
        {
            var settingsEntity = _context.UsersSettings
               .Include(u => u.UserPatterns)
               .Include(u => u.SharedCustomUserElements)
               .Single(u => u.UserId == userId);

            settingsEntity.UserPatterns.ForEach(u => u.CustomUserElementsForPattern = _context.CustomUserElements.Where(el => el.UserPatternId == u.Id).ToList());

            var userSettings = new
            {
                UserPatterns = settingsEntity.UserPatterns,
                SharedCustomUserElements = settingsEntity.SharedCustomUserElements.Where(i => i.UserPatternId == null),
                TextLengthBeforeCut = settingsEntity.TextLengthBeforeCut,
                CutElement = settingsEntity.CutElement,
                ParagraphElement = settingsEntity.ParagraphElement,
                ParagraphCenterAlignClass = settingsEntity.ParagraphCenterAlignClass,
                ListElement = settingsEntity.ListElement,
            };

            return userSettings;
        }

        public async Task UpdateUserSettings(UpdatedSettings updatedSettings, int userId)
        {
            if (updatedSettings.AddedElements?.Count() > 1000 ||
                updatedSettings.RemovedElements?.Count() > 1000 ||
                updatedSettings.UpdatedElements?.Count() > 1000) throw new Exception("too many elements.");

            var settings = _context.UsersSettings
                .AsNoTracking()
                .Single(s => s.UserId == userId);

            updatedSettings.UserSettingsEntity.Id = settings.Id;
            updatedSettings.UserSettingsEntity.UserId = userId;

            _context.UsersSettings.Update(updatedSettings.UserSettingsEntity);

            if (updatedSettings.AddedElements != null)
            {
                foreach (var element in updatedSettings.AddedElements) element.UserSettingsEntityId = settings.Id;
                _context.CustomUserElements.AddRange(updatedSettings.AddedElements);
            }

            if (updatedSettings.RemovedElements != null)
                _context.CustomUserElements.RemoveRange(updatedSettings.RemovedElements);

            if (updatedSettings.UpdatedElements != null)
                _context.CustomUserElements.UpdateRange(updatedSettings.UpdatedElements);

            await _context.SaveChangesAsync();
        }
    }
}
