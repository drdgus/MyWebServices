using MyWebServices.Core.Extensions;
using MyWebServices.Core.Models;
using NPOI.XWPF.UserModel;
using System.Text;

namespace MyWebServices.Core.Services
{
    public class WordManager
    {
        private XWPFDocument _wordDocument = new();

        public WordManager(Stream stream)
        {
            stream.Position = 0;
            _wordDocument = new XWPFDocument(stream);

            //Использовать IEnumerator 
        }

        public string GetCovertedText()
        {
            var convertedText = new StringBuilder();

            convertedText.AppendLine(Settings.PreviewImageElement);

            _wordDocument.BodyElements.ToList().ForEach(el =>
            {
                convertedText.AppendLine(ConvertElement(el));
            });

            convertedText.AppendLine(Settings.GalleryElement);

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

        private ConvertedParagraphsInfo _convertedParagraphsInfo = new();
        private string ConvertParagraph(XWPFParagraph paragraph)
        {
            if (paragraph.IsEmpty) return string.Empty;
            if (paragraph.ParagraphText.Length == 0) return string.Empty;

            var strBulder = new StringBuilder();

            if(
                (_convertedParagraphsInfo.CutElementInserted == false &&
                ParagraphIsList(paragraph) &&
                _convertedParagraphsInfo.LastIsNumbering == false) 
                ||
                (_convertedParagraphsInfo.CutElementInserted == false &&
                _convertedParagraphsInfo.TextLength + paragraph.ParagraphText.Length >= Settings.TextLengthBeforeCut && 
                _convertedParagraphsInfo.LastIsNumbering == false)
              )
            {
                _convertedParagraphsInfo.CutElementInserted = true;
                strBulder.AppendLine(Settings.CutElement);
            }

            _convertedParagraphsInfo.Count++;
            _convertedParagraphsInfo.TextLength += paragraph.ParagraphText.Length;

            var alignment = GetAlignment(paragraph.Alignment);

            if(TryConvertToList(paragraph, out string text))
            {
                strBulder.Append(text); //<ul...> // <li>{paragraph.ParagraphText}</li> // </ul...>

                if (_convertedParagraphsInfo.LastIsNumbering == false)
                {
                    strBulder.Append(GetText());
                    return strBulder.ToString();
                }
            }
            else
            {
                strBulder.Append(GetText());
            }

            return strBulder.ToString();

            string GetText()
            {
                return Settings.CreateParagraph(paragraph.ParagraphText, alignment);
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

        private bool TryConvertToList(XWPFParagraph paragraph, out string text)
        {
            var strBulder = new StringBuilder();
            var isParagraphList = ParagraphIsList(paragraph);
            
            if (paragraph.GetNumFmt() != null)
            {
                _convertedParagraphsInfo.LastListType = paragraph.GetNumFmt().Contains("bullet") ? "ul" : "ol";
            }

            if (isParagraphList == false && _convertedParagraphsInfo.LastIsNumbering == false)
            {
                text = string.Empty;
                return false;
            }

            if (isParagraphList == true)
            {
                if (_convertedParagraphsInfo.LastIsNumbering == false)
                {
                    _convertedParagraphsInfo.LastIsNumbering = true;
                    strBulder.AppendLine($"<{Settings.GetListHeader(_convertedParagraphsInfo.LastListType)}>");
                }
                strBulder.Append($"<li>{paragraph.ParagraphText}</li>");
            }

            if (_convertedParagraphsInfo.LastIsNumbering && isParagraphList == false)
            {
                strBulder.AppendLine($"</{_convertedParagraphsInfo.LastListType}>");
                _convertedParagraphsInfo.LastIsNumbering = false;
            }

            text = strBulder.ToString();

            return true;
        }

        private bool ParagraphIsList(XWPFParagraph paragraph)
        {
            return !string.IsNullOrWhiteSpace(paragraph.GetNumFmt());
        }

        private string GetAlignment(ParagraphAlignment alignment)
        {
            return alignment switch
            {
                ParagraphAlignment.LEFT => string.Empty,
                ParagraphAlignment.CENTER => Settings.ParagraphCenterAlignClass,
                ParagraphAlignment.RIGHT => string.Empty,
                ParagraphAlignment.BOTH => string.Empty,
                _ => alignment.ToString()
            };
        }
    }
}
