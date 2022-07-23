namespace MyWebServices.Core.DataAccess.Entities
{
    public class UserSettingsEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public List<UserPattern> UserPatterns { get; set; }
        public List<CustomUserElement> SharedCustomUserElements { get; set; }

        public int TextLengthBeforeCut { get; set; }
        public string CutElement { get; set; }

        public string ParagraphElement { get; set; }
        public string ParagraphCenterAlignClass { get; set; }

        public string ListElement { get; set; }
    }
}
