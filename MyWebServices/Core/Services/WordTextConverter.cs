using System.Text;
using MyWebServices.Core.Models;

namespace MyWebServices.Core.Services
{
    public class WordTextConverter
    {
        private readonly UserSettings _userSettings;
        private TextStyle? _lastContentStyle = null;

        public WordTextConverter(UserSettings userSettings)
        {
            _userSettings = userSettings;
        }

        public string Convert(IList<MsWordItem> wordItems)
        {
            var convertedText = new StringBuilder();

            var depth = 0;
            for (var i = 0; i < wordItems.Count; i++)
            {
                var wordItem = wordItems[i];

                if (wordItem.ItemType == ItemType.Paragraph)
                    convertedText.AppendLine(ConvertToParagraph(wordItem));
                else
                {
                    if (i == 0)
                    {
                        convertedText.AppendLine($"<{GetMsListSign((MsWordList)wordItem)}>");
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                        continue;
                    }

                    if (wordItems[i - 1].ItemType == ItemType.Paragraph)
                    {
                        convertedText.AppendLine($"<{GetMsListSign((MsWordList)wordItem)}>");
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                        continue;
                    }

                    if (i + 1 == wordItems.Count)
                    {
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                        for (int j = 0; j <= depth; j++)
                        {
                            convertedText.AppendLine($"</{GetMsListSign((MsWordList)wordItem)}>");
                        }
                        continue;
                    }

                    if (wordItems[i + 1].ItemType == ItemType.Paragraph)
                    {
                        convertedText.AppendLine(ConvertToListItem(wordItem));

                        for (int j = 0; j <= depth; j++)
                        {
                            convertedText.AppendLine($"</{GetMsListSign((MsWordList)wordItem)}>");
                        }
                        continue;
                    }

                    if (((MsWordList)wordItems[i + 1]).Depth > ((MsWordList)wordItem).Depth)
                    {
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                        convertedText.AppendLine($"<{GetMsListSign((MsWordList)wordItems[i + 1])}>");
                        depth++;
                    }
                    else if (((MsWordList)wordItems[i + 1]).Depth < ((MsWordList)wordItem).Depth)
                    {
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                        convertedText.AppendLine($"</{GetMsListSign((MsWordList)wordItem)}>");
                        depth--;
                    }
                    else
                    {
                        convertedText.AppendLine(ConvertToListItem(wordItem));
                    }
                }
            }

            return convertedText.ToString();
        }

        private string ConvertToParagraph(MsWordItem wordItem)
        {
            //_userSettings.ParagraphElement;
            return $"<p>{ConvertItemContent(wordItem)}</p>";
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
                        strBuilder.Append($"{textElement.Text}</{GetTextStyleSign(textElement.Style)}>");

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
    }
}
