using Microsoft.EntityFrameworkCore;
using MyWebServices.Core.DataAccess.Entities;
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
                SharedCustomUserElements = settingsEntity.SharedCustomUserElements,
                TextLengthBeforeCut = settingsEntity.TextLengthBeforeCut,
                CutElement = settingsEntity.CutElement,
                ParagraphElement = settingsEntity.ParagraphElement,
                ParagraphCenterAlignClass = settingsEntity.ParagraphCenterAlignClass,
                ListElement = settingsEntity.ListElement,
            };

            return userSettings;
        }

        public async Task UpdateUserSettings(UserSettingsEntity userSettingsEntity, int userId)
        {
            var settings = _context.UsersSettings.AsNoTracking().Single(u => u.Id == userId);
            userSettingsEntity.UserId = settings.UserId;
            userSettingsEntity.Id = settings.Id;

            _context.UsersSettings.Update(userSettingsEntity);
            await _context.SaveChangesAsync();

            //var settings = _context.UsersSettings.AsNoTracking()
            //    .Include(i => i.UserPatterns)
            //    .Include(u => u.SharedCustomUserElements)
            //    .Single(u => u.Id == userId);

            //settings.UserPatterns
            //    .ForEach(u => u.CustomUserElementsForPattern = _context.CustomUserElements
            //        .Where(el => el.UserPatternId == u.Id)
            //        .ToList());

            //userSettingsEntity.UserId = settings.UserId;
            //userSettingsEntity.Id = settings.Id;

            //var newSharedCustomElements =  userSettingsEntity.SharedCustomUserElements.Where(i =>
            //    settings.SharedCustomUserElements.Contains(i) == false);

            //var oldUserPatterns = settings.UserPatterns;
            //var newUserPatterns = userSettingsEntity.UserPatterns;

            //var newPatternsCustomElements = new List<CustomUserElement>();

            //for (var i = 0; i < oldUserPatterns.Count; i++)
            //{
            //    var newElements = newUserPatterns[i].CustomUserElementsForPattern
            //        .Where(elem => oldUserPatterns[i].CustomUserElementsForPattern
            //            .Contains(elem) == false);
            //    newPatternsCustomElements.AddRange(newElements);
            //}

            //await _context.CustomUserElements.AddRangeAsync(newSharedCustomElements);
            //await _context.CustomUserElements.AddRangeAsync(newPatternsCustomElements);

            //settings = _context.UsersSettings.AsNoTracking().Single(u => u.Id == userId);
            //userSettingsEntity.SharedCustomUserElements = settings.SharedCustomUserElements;
            //userSettingsEntity.UserPatterns = settings.UserPatterns;

            //_context.UsersSettings.Update(userSettingsEntity);
            //await _context.SaveChangesAsync();
        }
    }
}
