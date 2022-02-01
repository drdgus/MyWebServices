using NPOI.XWPF.UserModel;
using System.Text;

namespace MyWebServices.Core.Extensions
{
    public static class WordExtensions
    {
        public static string ParagraphsToString(this IList<XWPFParagraph> xWPFParagraphs)
        {
            var strBulder = new StringBuilder();
            xWPFParagraphs.ToList().ForEach(el => strBulder.Append(el.ParagraphText));
            return strBulder.ToString();
        }
    }
}
