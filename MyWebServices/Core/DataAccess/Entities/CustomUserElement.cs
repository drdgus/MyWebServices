namespace MyWebServices.Core.DataAccess.Entities
{
    public class CustomUserElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string? TemplateValue { get; set; }
        public SortingOrder ElementSortingOrder { get; set; }
        public int? UserPatternId { get; set; }
        public int? UserSettingsEntityId { get; set; }

        public enum SortingOrder
        {
            BeforeText,
            AfterText,
            Template
        }
    }
}
