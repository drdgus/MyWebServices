using MyWebServices.Core.DataAccess.Entities;
using System.Text;
using System.Text.RegularExpressions;

namespace MyWebServices.Core.Models
{
    public class UserSettings
    {
        public UserPattern UserPattern { get; set; }
        public List<CustomUserElement> SharedCustomUserElements { get; set; }

        public int TextLengthBeforeCut { get; set; }
        public string CutElement { get; set; }

        public string ParagraphElement { get; set; }
        public string ParagraphCenterAlignClass { get; set; }

        public string ListElement { get; set; }

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

            paragraph = Regex.Replace(paragraph, @"{\$ParagraphText\$}", "--new Paragraph Text--");

            return paragraph.Replace("--new Paragraph Text--", text);
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
            var elements = new StringBuilder();
            UserPattern.CustomUserElementsForPattern.ForEach(el =>
            {
                if (el.ElementSortingOrder == CustomUserElement.SortingOrder.BeforeText) elements.AppendLine(el.Value);
            });

            SharedCustomUserElements.ForEach(el =>
            {
                if (el.ElementSortingOrder == CustomUserElement.SortingOrder.BeforeText) elements.AppendLine(el.Value);
            });
            return elements.ToString();
        }

        public string GetElementsAfterText()
        {
            var elements = new StringBuilder();
            UserPattern.CustomUserElementsForPattern.ForEach(el =>
            {
                if (el.ElementSortingOrder == CustomUserElement.SortingOrder.AfterText) elements.AppendLine(el.Value);
            });

            SharedCustomUserElements.ForEach(el =>
            {
                if (el.ElementSortingOrder == CustomUserElement.SortingOrder.AfterText) elements.AppendLine(el.Value);
            });
            return elements.Remove(elements.Length - 2, 2).ToString();
        }

        public List<TemplateElement> GetTemplateElements()
        {
            var elements = new List<TemplateElement>();
            UserPattern.CustomUserElementsForPattern.ForEach(el =>
            {
                if (el.ElementSortingOrder == null)
                {
                    elements.Add(
                        new TemplateElement
                        {
                            Value = el.Value,
                            TemplateValue = el.TemplateValue
                        });
                }
            });

            return elements;
        }
    }
}
