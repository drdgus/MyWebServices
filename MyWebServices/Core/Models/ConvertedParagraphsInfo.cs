namespace MyWebServices.Core.Models
{
    public class ConvertedParagraphsInfo
    {
        public int Count { get; set; }
        public int TextLength { get; set; }
        public bool CutElementInserted { get; set; }
        public bool LastIsNumbering { get; set; }
        public string LastListType { get; set; }
    }
}
