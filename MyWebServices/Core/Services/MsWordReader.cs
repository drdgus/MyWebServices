using MyWebServices.Core.Models;
using NPOI.XWPF.UserModel;

namespace MyWebServices.Core.Services
{
    public class MsWordReader
    {
        private readonly List<MsWordItem> _wordItems = new();

        private XWPFDocument _document;
        private readonly Dictionary<string, int> _lastListDepth = new();

        public IList<MsWordItem> GetContent(Stream wordDocument)
        {
            _document = new XWPFDocument(wordDocument);

            ParseContent();

            return _wordItems;
        }

        private void ParseContent()
        {
            var bodyElements = _document.BodyElements;

            foreach (var bodyElement in bodyElements)
            {
                MsWordItem item;
                if (bodyElement.ElementType == BodyElementType.TABLE)
                {
                    var table = (XWPFTable)bodyElement;
                    item = new MsWordItem()
                    {
                        ItemType = ItemType.Table,
                        Content = GetTableContent(table),
                        Alignment = ContentAlignment.Left,
                    };
                    _wordItems.Add(item);
                    continue;
                }

                if (bodyElement.ElementType != BodyElementType.PARAGRAPH) continue;

                var paragraph = (XWPFParagraph)bodyElement;

                if (paragraph.IRuns.Count == 0) continue;


                if (paragraph.NumLevelText == null)
                {
                    item = new MsWordItem
                    {
                        ItemType = ItemType.Paragraph,
                        Content = GetParagraphContent(paragraph),
                        Alignment = GetAlignment(paragraph)
                    };
                }
                else
                {
                    item = new MsWordList()
                    {
                        ItemType = ItemType.List,
                        Content = GetParagraphContent(paragraph),
                        Alignment = GetAlignment(paragraph),
                        Depth = GetListItemDepth(paragraph),
                        DepthSign = paragraph.NumLevelText
                    };
                }

                _wordItems.Add(item);
            }
        }

        private List<TextElement> GetParagraphContent(XWPFParagraph paragraph)
        {
            var textElements = new List<TextElement>();

            foreach (var run in paragraph.Runs)
            {
                TextStyle style;

                if (run.IsBold) style = TextStyle.Bold;
                else if (run.IsItalic) style = TextStyle.Italic;
                else style = TextStyle.Normal;

                textElements.Add(new TextElement
                {
                    Style = style,
                    Text = run.Text
                });
            }

            var first = textElements.First();
            var last = textElements.Last();

            if (first.Text == "\t")
                textElements.Remove(first);

            first = textElements.First();
            first.Text = first.Text.TrimStart();

            if (last.Text == "\t")
                textElements.Remove(last);

            last = textElements.Last();
            last.Text = last.Text.TrimEnd();

            return textElements;
        }

        private List<TextElement> GetTableContent(XWPFTable table)
        {
            var textElements = new List<TextElement>();
            var text = table.Text.Split('\n').SkipLast(1);
            foreach (var item in text)
            {
                textElements.Add(new TextElement
                {
                    Style = TextStyle.Normal,
                    Text = item
                });
            }

            return textElements;
        }

        private ContentAlignment GetAlignment(XWPFParagraph paragraph)
        {   return paragraph.Alignment switch
            {
                ParagraphAlignment.LEFT => ContentAlignment.Left,
                ParagraphAlignment.CENTER => ContentAlignment.Center,
                ParagraphAlignment.RIGHT => ContentAlignment.Right,
                ParagraphAlignment.BOTH => ContentAlignment.Both,
                _ => ContentAlignment.Left
            };
        }

        private int GetListItemDepth(XWPFParagraph paragraph)
        {
            _lastListDepth.TryAdd(paragraph.NumLevelText, _lastListDepth.Count);

            if (_wordItems.Count == 0) return 0;

            var lastItem = _wordItems.Last();

            if (lastItem.ItemType == ItemType.Paragraph) return 0;

            if (((MsWordList)lastItem).DepthSign == paragraph.NumLevelText)
                return ((MsWordList)lastItem).Depth;

            if (_lastListDepth.ContainsKey(paragraph.NumLevelText))
                return _lastListDepth[paragraph.NumLevelText];

            return ++((MsWordList)lastItem).Depth;
        }
    }
}
