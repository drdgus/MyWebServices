using System.Text.RegularExpressions;

namespace MyWebServices.Core.Models
{
    public static class Settings
    {
        public static int TextLengthBeforeCut { get; set; } = 400;
        public static string CutElement { get; set; } = "<div class=''>$CUT$</div>";

        public static string ParagraphElement { get; set; } = "<p class='text'>{$ParagraphText$}</p>";
        public static string ParagraphCenterAlignClass { get; set; } = "centertext";
        
        public static string ListElement { get; set; } = "<{$list$} class='list'>";


        public static string PreviewImageElement { get; set; } = "<img class='photo' src='https://44-563-webapps-f21.github.io/webapps-f21-assignment-6-AbdulRehmanSayeed/owl.png' alt='owlImg'>";
        public static string GalleryElement { get; set; } = "<div class='Gallery'><a href='ссылка на альбом'>Фотоальбом</a></div>";

        /// <summary>
        /// Формирование тега абзаца по шаблону пользователя
        /// </summary>
        /// <param name="classes"></param>
        /// <param name="text"></param>
        /// <returns>тег абзаца с параметрами и текстом</returns>
        public static string CreateParagraph(string text, string classes = default)
        {
            var paragraph = ParagraphElement;
            if(string.IsNullOrWhiteSpace(classes) == false)
                paragraph = Regex.Replace(paragraph, "class='*'", $"class='{classes} ");

            paragraph = Regex.Replace(paragraph, @"{\$ParagraphText\$}", text);
            
            return paragraph;
        }

        /// <summary>
        /// Формирование тега списка по шаблону пользователя
        /// </summary>
        /// <param name="listType">тип списка, нумерованный, маркированный и т.д.</param>
        /// <returns>параметры открывающей части тега списка</returns>
        public static string GetListHeader(string listType)
        {
            var listHeader = string.Empty;
            listHeader = Regex.Match(ListElement, @"{\$list\$} class='.*'").Value;
            listHeader = Regex.Replace(listHeader, @"{\$list\$}", listType);
            return listHeader;
        }
    }
}
