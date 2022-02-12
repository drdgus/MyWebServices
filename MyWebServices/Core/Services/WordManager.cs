using MyWebServices.Core.Models;
using NPOI.XWPF.UserModel;
using System.Text;

namespace MyWebServices.Core.Services
{
    public class WordManager
    {
        private readonly XWPFDocument _wordDocument;
        private readonly ConvertedParagraphsInfo _convertedParagraphsInfo = new();
        private readonly UserSettings _userSettings;

        public WordManager(Stream stream, UserSettings userSettings)
        {
            _userSettings = userSettings;
            stream.Position = 0;
            _wordDocument = new XWPFDocument(stream);
        }

        public string GetConvertedText()
        {
            var convertedText = new StringBuilder();
            convertedText.AppendLine(_userSettings.GetElementsBeforeText());

            _wordDocument.BodyElements
                .ToList()
                .ForEach(el => convertedText.AppendLine(ConvertElement(el)));

            convertedText.AppendLine(_userSettings.GetElementsAfterText());

            _userSettings.GetTemplateElements().ForEach(templateEl => convertedText.Replace(templateEl.TemplateValue, templateEl.Value));

            return convertedText.ToString();
        }

        private string ConvertElement(IBodyElement el)
        {
            var strBuilder = new StringBuilder();
            _ = el.ElementType switch
            {
                BodyElementType.CONTENTCONTROL => throw new NotImplementedException(nameof(el.ElementType)),
                BodyElementType.PARAGRAPH => strBuilder.Append(ConvertParagraph((XWPFParagraph)el)),
                BodyElementType.TABLE => strBuilder.AppendLine(ConvertTable((XWPFTable)el)),
                _ => throw new ArgumentOutOfRangeException(nameof(el.ElementType), "Такой тип не предусмотрен библиотекой работающей с word.")
            };

            return strBuilder.ToString();
        }

        private string ConvertParagraph(XWPFParagraph paragraph)
        {
            if (paragraph.IsEmpty) return string.Empty;
            if (paragraph.ParagraphText.Length == 0) return string.Empty;

            var strBuilder = new StringBuilder();

            if (IsCutElementNotInserted(paragraph))
            {
                _convertedParagraphsInfo.CutElementInserted = true;
                strBuilder.AppendLine(_userSettings.CutElement);
            }

            _convertedParagraphsInfo.Count++;
            _convertedParagraphsInfo.TextLength += paragraph.ParagraphText.Length;

            var alignment = GetAlignment(paragraph.Alignment);

            if (TryConvertToList(paragraph, out string convertedListElement))
            {
                strBuilder.Append(convertedListElement);

                if (_convertedParagraphsInfo.IsLastNumbering == false)
                {
                    strBuilder.Append(GetText());
                    return strBuilder.ToString();
                }
            }
            else strBuilder.Append(GetText());

            return strBuilder.ToString();

            string GetText()
            {
                return _userSettings.CreateParagraph(paragraph.ParagraphText, alignment);
            }
        }

        private string ConvertTable(XWPFTable table)
        {
            var strBuilder = new StringBuilder();
            table.Rows
                .ForEach(row => row.GetTableCells()
                    .ForEach(cell => cell.BodyElements.ToList()
                        .ForEach(el => strBuilder.AppendLine(ConvertElement(el)))));
            return strBuilder.ToString();
        }

        private bool IsCutElementNotInserted(XWPFParagraph paragraph)
        {
            if (IsParagraphList(paragraph) &&
                _convertedParagraphsInfo.CutElementInserted == false &&
                _convertedParagraphsInfo.IsLastNumbering == false)
            {
                return true;
            }
            else if (_convertedParagraphsInfo.CutElementInserted == false &&
                    _convertedParagraphsInfo.TextLength + paragraph.ParagraphText.Length >= _userSettings.TextLengthBeforeCut &&
                    _convertedParagraphsInfo.IsLastNumbering == false)
            {
                return true;
            }
            return false;
        }

        private bool TryConvertToList(XWPFParagraph paragraph, out string text)
        {
            var strBuilder = new StringBuilder();
            var isParagraphList = this.IsParagraphList(paragraph);

            if (paragraph.GetNumFmt() != null)
            {
                _convertedParagraphsInfo.LastListNumberingType = paragraph.GetNumFmt().Contains("bullet") ? "ul" : "ol";
            }

            if (isParagraphList == false && _convertedParagraphsInfo.IsLastNumbering == false)
            {
                text = string.Empty;
                return false;
            }

            if (isParagraphList == true)
            {
                if (_convertedParagraphsInfo.IsLastNumbering == false)
                {
                    _convertedParagraphsInfo.IsLastNumbering = true;
                    strBuilder.AppendLine($"<{_userSettings.GetListHeader(_convertedParagraphsInfo.LastListNumberingType)}>");
                }
                strBuilder.Append($"<li>{paragraph.ParagraphText}</li>");
            }

            if (_convertedParagraphsInfo.IsLastNumbering && isParagraphList == false)
            {
                strBuilder.AppendLine($"</{_convertedParagraphsInfo.LastListNumberingType}>");
                _convertedParagraphsInfo.IsLastNumbering = false;
            }

            text = strBuilder.ToString();  //<ul...> // <li>{paragraph.ParagraphText}</li> // </ul...>

            return true;
        }

        private bool IsParagraphList(XWPFParagraph paragraph)
        {
            return !string.IsNullOrWhiteSpace(paragraph.GetNumFmt());
        }

        private string GetAlignment(ParagraphAlignment alignment)
        {
            return alignment switch
            {
                ParagraphAlignment.LEFT => string.Empty,
                ParagraphAlignment.CENTER => _userSettings.ParagraphCenterAlignClass,
                ParagraphAlignment.RIGHT => string.Empty,
                ParagraphAlignment.BOTH => string.Empty,
                _ => alignment.ToString()
            };
        }
    }
}
