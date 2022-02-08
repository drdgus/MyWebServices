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
                UserPattern = settingsEntity.UserPatterns,
                SharedCustomUserElements = settingsEntity.SharedCustomUserElements,
                CutElement = settingsEntity.CutElement,
                TextLengthBeforeCut = settingsEntity.TextLengthBeforeCut,
                ParagraphElement = settingsEntity.ParagraphElement,
                ParagraphCenterAlignClass = settingsEntity.ParagraphCenterAlignClass,
                ListElement = settingsEntity.ListElement,
            };

            return userSettings;
        }
    }
}
