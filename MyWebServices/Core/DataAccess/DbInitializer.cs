﻿using MyWebServices.Core.DataAccess.Entities;

namespace MyWebServices.Core.DataAccess
{
    public static class DbInitializer
    {
        public static void Initialize(ParagraphsDbContext context)
        {
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User{ Login = "test1", PasswordHash = "" },
                new User{ Login = "test2", PasswordHash = "" },
                new User{ Login = "test3", PasswordHash = "" },
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            var settings = new UserSettingsEntity[]
            {
                new UserSettingsEntity
                {
                    UserId = 1,
                    UserPatterns = new List<UserPattern>
                    {
                        new UserPattern
                        {
                            Name = "Без доп. элементов"
                        },
                        new UserPattern
                        {
                            Name = "Пожарная безопасность",
                            CustomUserElementsForPattern = new List<CustomUserElement>
                            {
                                new CustomUserElement
                                {
                                    Name = "Ссылка на оф сайт пожарной части Богучан",
                                    Value = "URL TO Firehouse Site before text.",
                                    ElementSotringOrder = CustomUserElement.SortingOrder.BeforeText
                                },
                                new CustomUserElement
                                {
                                    Name = "Ссылка на документы школы",
                                    TemplateValue = "$$Documents",
                                    Value = "URL TO Documents section."
                                }
                            }
                        }
                    },
                    SharedCustomUserElements = new List<CustomUserElement>
                    {
                        new CustomUserElement
                        {
                            Name = "Картинка перед текстом",
                            ElementSotringOrder = CustomUserElement.SortingOrder.BeforeText,
                            Value = "<img class='photo' src='https://44-563-webapps-f21.github.io/webapps-f21-assignment-6-AbdulRehmanSayeed/owl.png' alt='owlImg'>"
                        },
                        new CustomUserElement
                        {
                            Name = "Альбом",
                            ElementSotringOrder = CustomUserElement.SortingOrder.AfterText,
                            Value = "<div class='Gallery'><a href='ссылка на альбом'>Фотоальбом</a></div>"
                        }
                    },
                    TextLengthBeforeCut = 400,
                    CutElement = "<div class=''>$CUT$</div>",
                    ParagraphElement  = "<p class='text'>{$ParagraphText$}</p>",
                    ParagraphCenterAlignClass = "centertext",
                    ListElement  = "<{$list$} class='list'>",
                }
            };

            context.UsersSettings.AddRange(settings);
            context.SaveChanges();
        }
    }
}
