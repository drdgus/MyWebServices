using MyWebServices.Core.Models;

namespace MyWebServices.Core.Models
{
    public class MsWordItem
    {
        public ItemType ItemType { get; init; }
        public List<TextElement> Content { get; set; }
        public ContentAlignment Alignment { get; set; }

        public int ContentLength => GetContentLength();

        private int GetContentLength()
        {
            var length = 0;
            Content.ForEach(i => length += i.Text.Length);
            return length;
        }
    }
}
