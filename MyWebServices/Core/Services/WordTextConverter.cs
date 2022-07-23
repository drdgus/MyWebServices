using MyWebServices.Core.Models;
using System.Text;

namespace MyWebServices.Core.Services
{
    public class WordTextConverter
    {
        private readonly UserSettings _userSettings;

        private readonly StringBuilder _convertedTextBuilder = new();
        private TextStyle? _lastContentStyle;

        public WordTextConverter(UserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public string Convert(IList<MsWordItem> wordItems)
        {
            SetUserElementsBeforeText();
            ConvertText(wordItems);
            SetUserElementsAfterText();
            ReplaceUserTemplateElements();

            return _convertedTextBuilder.ToString();
        }

        private void SetUserElementsBeforeText()
        {
            _convertedTextBuilder.Append(GetElementsBeforeText());
        }

        private void SetUserElementsAfterText()
        {
            _convertedTextBuilder.Append(GetElementsAfterText());
        }

        private void ReplaceUserTemplateElements()
        {
            _userSettings.GetTemplateElements().ForEach(templateEl =>
            {
                _convertedTextBuilder.Replace(templateEl.TemplateValue, templateEl.Value);
            });
        }

        private void ConvertText(IList<MsWordItem> wordItems)
        {
            var cutElementInserted = false;
            var currentContentLength = 0;
            var listDepth = new List<string>();
            var firstListItem = true;

            for (var i = 0; i < wordItems.Count; i++)
            {
                var wordItem = wordItems[i];

                if (wordItem.ItemType == ItemType.Paragraph || wordItem.ItemType == ItemType.Table)
                {
                    if (wordItem.ItemType == ItemType.Paragraph)
                        _convertedTextBuilder.AppendLine(ConvertToParagraph(wordItem));
                    else _convertedTextBuilder.AppendLine(ConvertToTable(wordItem));

                    if (cutElementInserted == false)
                    {
                        currentContentLength += wordItem.ContentLength;

                        if (i + 1 != wordItems.Count && currentContentLength + wordItems[i + 1].ContentLength >= _userSettings.TextLengthBeforeCut)
                        {
                            cutElementInserted = true;
                            _convertedTextBuilder.AppendLine(_userSettings.CutElement);
                        }
                    }

                    listDepth.Clear();
                    firstListItem = true;
                }
                else
                {
                    var listItem = (MsWordList)wordItem;

                    if (listItem.Depth == listDepth.Count)
                    {
                        SetListItem(listItem, ref firstListItem);
                    }
                    else if (listItem.Depth > listDepth.Count)
                    {
                        firstListItem = true;
                        listDepth.Add(GetMsListSign(listItem));

                        SetListItem(listItem, ref firstListItem);
                    }
                    else if (listItem.Depth < listDepth.Count)
                    {
                        for (int maxDepth = listDepth.Count - 1; maxDepth >= 0; maxDepth--)
                        {
                            if (listItem.Depth < maxDepth + 1)
                            {
                                _convertedTextBuilder.AppendLine($"</{listDepth[maxDepth]}>");
                                listDepth.RemoveAt(maxDepth);
                            }
                        }

                        SetListItem(listItem, ref firstListItem);
                    }

                    if (i + 1 == wordItems.Count || wordItems[i + 1].ItemType == ItemType.Paragraph)
                    {
                        for (var j = 0; j < listDepth.Count + 1; j++)
                        {
                            if (j + 1 > listDepth.Count)
                            {
                                _convertedTextBuilder.AppendLine($"</{GetMsListSign(listItem)}>");
                                continue;
                            }

                            var listSign = listDepth[j];
                            _convertedTextBuilder.AppendLine($"</{listSign}>");
                        }
                    }
                }
            }
        }

        private string GetElementsBeforeText()
        {
            return _userSettings.GetElementsBeforeText();
        }

        private string GetElementsAfterText()
        {
            return _userSettings.GetElementsAfterText();
        }

        private void SetListItem(MsWordList listItem, ref bool firstListItem)
        {
            if (firstListItem)
            {
                firstListItem = false;
                _convertedTextBuilder.AppendLine($"{GetMsListOpeningTag(listItem)}{Environment.NewLine}{ConvertToListItem(listItem)}");
            }
            else _convertedTextBuilder.AppendLine(ConvertToListItem(listItem));
        }

        private string ConvertToParagraph(MsWordItem wordItem)
        {
            //_userSettings.ParagraphElement;
            var alignmentClass = GetAlignmentClass(wordItem.Alignment);
            return _userSettings.CreateParagraph(ConvertItemContent(wordItem), alignmentClass);
        }

        private string ConvertToTable(MsWordItem wordItem)
        {
            var table = new StringBuilder();
            table.AppendLine("<table>");

            wordItem.Content.ForEach(row =>
            {
                table.AppendLine("<tr>");
                row.Text.Split('\t').ToList().ForEach(cell => table.AppendLine($"<td>{cell}</td>"));
                table.AppendLine("</tr>");
            });

            table.Append("</table>");

            return table.ToString();
        }

        private string ConvertToListItem(MsWordItem wordItem)
        {
            return $"<li>{ConvertItemContent(wordItem)}</li>";
        }

        private string ConvertItemContent(MsWordItem item)
        {
            var strBuilder = new StringBuilder();

            for (var i = 0; i < item.Content.Count; i++)
            {
                var textElement = item.Content[i];

                if (textElement.Style == TextStyle.Normal)
                {
                    if (_lastContentStyle != null && _lastContentStyle != TextStyle.Normal)
                    {
                        strBuilder.Append($"</{GetTextStyleSign((TextStyle)_lastContentStyle!)}>");
                        _lastContentStyle = textElement.Style;
                    }
                    strBuilder.Append(textElement.Text);
                }
                else
                {
                    if (_lastContentStyle == null)
                    {
                        strBuilder.Append($"<{GetTextStyleSign(textElement.Style)}>{textElement.Text}");
                    }
                    else if (_lastContentStyle != textElement.Style)
                    {
                        if (_lastContentStyle != TextStyle.Normal)
                            strBuilder.Append($"</{GetTextStyleSign((TextStyle)_lastContentStyle!)}>");
                        strBuilder.Append($"<{GetTextStyleSign(textElement.Style)}>{textElement.Text}");
                    }

                    else
                    {
                        strBuilder.Append(textElement.Text);
                    }

                    _lastContentStyle = textElement.Style;
                }

                if (item.Content.Count == i + 1)
                {
                    if (_lastContentStyle != null && _lastContentStyle != TextStyle.Normal)
                        strBuilder.Append($"</{GetTextStyleSign(textElement.Style)}>");

                    _lastContentStyle = null;
                }
            }

            return strBuilder.ToString();
        }

        private string GetTextStyleSign(TextStyle style)
        {
            return style switch
            {
                TextStyle.Bold => "b",
                TextStyle.Italic => "i",
                TextStyle.Normal => "",
                _ => ""
            };
        }

        private string GetMsListSign(MsWordList wordList)
        {
            return wordList.Type switch
            {
                ListType.Bullet => "ul",
                ListType.Number => "ol",
                _ => "ul"
            };
        }

        private string GetMsListOpeningTag(MsWordList wordList)
        {
            return $"<{_userSettings.GetListHeader(GetMsListSign(wordList))}>";
        }

        private string GetAlignmentClass(ContentAlignment alignment)
        {
            return alignment switch
            {
                ContentAlignment.Left => string.Empty,
                ContentAlignment.Center => _userSettings.ParagraphCenterAlignClass,
                ContentAlignment.Right => string.Empty,
                ContentAlignment.Both => string.Empty,
                _ => alignment.ToString()
            };
        }
    }
}
