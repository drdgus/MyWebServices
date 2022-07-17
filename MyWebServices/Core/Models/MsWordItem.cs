using MyWebServices.Core.Models;

namespace MyWebServices.Core.Models
{
    public class MsWordItem
    {
        public ItemType ItemType { get; init; }
        public List<TextElement> Content { get; set; }
        public ContentAlignment Alignment { get; set; }
    }

    public class MsWordParagraph : MsWordItem
    {

    }

    public class MsWordList : MsWordItem
    {
        public ListType Type { get; set; }
        public int Depth { get; set; }
        public MsWordList ChildList { get; set; }
        internal string DepthSign { get; init; }
    }

    public class TextElement
    {
        public TextStyle Style { get; set; }
        public string Text { get; set; }
    }

    public enum ItemType
    {
        Paragraph,
        List
    }

    public enum ListType
    {
        Bullet,
        Number
    }

    public enum ContentAlignment
    {
        Left,
        Center,
        Right,
        Both,
    }

    public enum TextStyle
    {
        Normal,
        Bold,
        Italic
    }
}
