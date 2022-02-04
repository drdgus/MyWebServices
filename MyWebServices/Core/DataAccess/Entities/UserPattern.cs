namespace MyWebServices.Core.DataAccess.Entities
{
    public class UserPattern
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CustomUserElement> CustomUserElementsForPattern { get; set; }
    }
}
