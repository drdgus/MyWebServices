using System.Text.RegularExpressions;

namespace MyWebServices.Core.DataAccess.Entities
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public List<UserPattern> UserPatterns { get; set; }
        public List<CustomUserElement> SharedCustomUserElements { get; set;  }

        public int TextLengthBeforeCut { get; set; }
        public string CutElement { get; set; }

        public string ParagraphElement { get; set; }
        public string ParagraphCenterAlignClass { get; set; }

        public string ListElement { get; set; }


        public string PreviewImageElement { get; set; }
        public string GalleryElement { get; set; }

        /// <summary>
        /// Формирование тега абзаца по шаблону пользователя
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="text"></param>
        /// <returns>тег абзаца с параметрами и текстом</returns>
        public string CreateParagraph(string text, string classes = default)
        {
            var paragraph = ParagraphElement;
            if (string.IsNullOrWhiteSpace(classes) == false)
                paragraph = Regex.Replace(paragraph, "class='*'", $"class='{classes} ");

            paragraph = Regex.Replace(paragraph, @"{\$ParagraphText\$}", text);

            return paragraph;
        }

        /// <summary>
        /// Формирование тега списка по шаблону пользователя
        /// </summary>
        /// <param name="listType">тип списка, нумерованный, маркированный и т.д.</param>
        /// <returns>параметры открывающей части тега списка</returns>
        public string GetListHeader(string listType)
        {
            var listHeader = string.Empty;
            listHeader = Regex.Match(ListElement, @"{\$list\$} class='.*'").Value;
            listHeader = Regex.Replace(listHeader, @"{\$list\$}", listType);
            return listHeader;
        }

        public string GetElementsBeforeText()
        {
            return PreviewImageElement;
        }

        public string GetElementsAfterText()
        {
            return GalleryElement;
        }
    }
}
