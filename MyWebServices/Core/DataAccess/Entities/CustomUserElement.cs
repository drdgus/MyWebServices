namespace MyWebServices.Core.DataAccess.Entities
{
    public class CustomUserElement
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string ReplaceValue { get; set; }
        public SortingOrder ElementSotringOrder { get; set; }

        public enum SortingOrder
        {
            BeforeText,
            AfterText
        }
    }
}
