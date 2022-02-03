namespace MyWebServices.Core.DataAccess.Entities
{
    public class UserSettings
    {
        public string[][] ElementsBeforeText { get; set; } = new string[][] { new string[] { "labels" },
                                                                              new string[] { "<span>labelText</span>" } };
        public string[][] ElementsAfterText { get; set; } = new string[][] { new string[] { "comments" },
                                                                             new string[] { "<input type='text'>lorem</input>" } };

        public int TextLengthBeforeCut { get; set; } = 400;
        public string CutElement { get; set; } = "<div class=''>$CUT$</div>";

        public string ParagraphElement { get; set; } = "<p class='text'>{$ParagraphText$}</p>";
        public string ParagraphCenterAlignClass { get; set; } = "centertext";

        public string ListElement { get; set; } = "<{$list$} class='list'>";


        public string PreviewImageElement { get; set; } = "<img class='photo' src='https://44-563-webapps-f21.github.io/webapps-f21-assignment-6-AbdulRehmanSayeed/owl.png' alt='owlImg'>";
        public string GalleryElement { get; set; } = "<div class='Gallery'><a href='ссылка на альбом'>Фотоальбом</a></div>";
    }
}
