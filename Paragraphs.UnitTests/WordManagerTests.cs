using MyWebServices.Core.DataAccess.Entities;
using MyWebServices.Core.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Paragraphs.UnitTests
{
    internal class WordManagerTests
    {
        private WordManager GetWordManager(Stream stream)
        {
            var userSettings = new UserSettings();
            userSettings.SharedCustomUserElements = new List<CustomUserElement>()
            {
                //new CustomUserElement
                //{
                //    Id = 0,
                //    Name = null,
                //    Value = null,
                //    TemplateValue = null,
                //    ElementSortingOrder = null,
                //    UserPatternId = null
                //}
            };

            userSettings.UserPattern = new UserPattern
            {
                Id = 0,
                Name = "Шаблон №1",
                CustomUserElementsForPattern = new List<CustomUserElement>
                {
                    //new CustomUserElement
                    //{
                    //    Id = 0,
                    //    Name = null,
                    //    Value = null,
                    //    TemplateValue = null,
                    //    ElementSortingOrder = null,
                    //    UserPatternId = null
                    //}
                }
            };

            userSettings.CutElement = "<div class=''>$CUT$</div>";
            userSettings.TextLengthBeforeCut = 400;
            userSettings.ListElement = "<{$list$} class='list'>";
            userSettings.ParagraphElement = "<p class='text'>{$ParagraphText$}</p>";
            userSettings.ParagraphCenterAlignClass = "centertext";

            return new WordManager(stream, userSettings);
        }

        [Test]
        public void GetConvertedText_OneParagraph()
        {
            //Arrange
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "OneParagraph.docx");
            using var fileToConvert = new FileInfo(filePath).Open(FileMode.Open);
            var expected = $"<p class='text'>Hello world!</p>";

            //Act
            var text = GetWordManager((Stream)fileToConvert).GetConvertedText();

            //Assert
            Assert.AreEqual(expected, text);
        }

        [Test]
        public void GetConvertedText_FewParagraphs()
        {
            //Arrange
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "FewParagraphs.docx");
            using var fileToConvert = new FileInfo(filePath).Open(FileMode.Open);
            var expected = $"<p class='text'>Hello world!</p>{Environment.NewLine}<p class='text'>Hello world!</p>";

            //Act
            var text = GetWordManager((Stream)fileToConvert).GetConvertedText();

            //Assert
            Assert.AreEqual(expected, text);
        }

        [Test]
        public void GetConvertedText_OneNumberingList()
        {
            //Arrange
            var filePath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "NumList.docx");
            using var fileToConvert = new FileInfo(filePath).Open(FileMode.Open);
            var expected = $"<div class=''>$CUT$</div>{Environment.NewLine}<ul class='list'>{Environment.NewLine}<li>Hello</li>{Environment.NewLine}<li>World!</li>{Environment.NewLine}</ul>";

            //Act
            var text = GetWordManager((Stream)fileToConvert).GetConvertedText();

            //Assert
            Assert.AreEqual(expected, text);
        }

        [Test]
        public void GetConvertedText_NumListTree()
        {

        }

        [Test]
        public void GetConvertedText_NumberingListAndBulletList()
        {

        }

        [Test]
        public void GetConvertedText_OneParagraph_TwoNumLists_OneTreeNumList()
        {

        }
    }
}
