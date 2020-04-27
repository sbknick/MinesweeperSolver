namespace MinesweeperSolver.WebDriver
{
    using MinesweeperSolver.Models;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using System;
    using System.Text.RegularExpressions;

    public class WebSquare : ISquare
    {
        //private bool isDirty;
        private bool isCleared;
        private bool? _isNumber;

        private IWebElement Element { get; }

        private string Class => this.Element.GetAttribute("class");
        private IAction SpaceMeUp { get; }

        private static readonly Regex NumberRegex = new Regex(@".* open(\d)");

        public WebSquare(IWebDriver driver, IWebElement element)
        {
            this.Element = element;

            var idStr = element.GetAttribute("id");
            this.Index = idStr.Split('_').ToIndex();
            this.IsBlank = true;

            this.SpaceMeUp = new Actions(driver)
                .MoveToElement(element)
                .SendKeys(Keys.Space)
                .Build();
        }

        public (int x, int y) Index { get; }

        public bool IsBlank { get; private set; }

        public bool IsFlagged { get; private set; }

        public bool IsNumber
        {
            get
            {
                if (this._isNumber.HasValue) return this._isNumber.Value;

                var result = this.TestIsNumber(out var number);
                if (result)
                {
                    this.IsNumber = true;
                    this.Number = number;
                }
                return result;
            }
            private set
            {
                if (value)
                {
                    this._isNumber = true;
                    this.IsBlank = false;
                    this.IsFlagged = false;
                }
                else this._isNumber = false;
            }
        }

        public int Number { get; private set; }

        public void Clear()
        {
            if (this.isCleared) return;
            if (this.IsBlank || this.IsFlagged) throw new NotSupportedException(); 

            if (this.Number != 0)
                this.SpaceMeUp.Perform();
            this.isCleared = true;
        }

        public void Click()
        {
            if (!this.IsBlank) throw new NotSupportedException();
           
            this.Element.Click();

            if (this.TestIsNumber(out var number))
            {
                this.IsNumber = true;
                this.Number = number;
            }
            else throw new NotSupportedException();
        }

        public void Flag()
        {
            if (!this.IsBlank) throw new NotSupportedException();

            this.SpaceMeUp.Perform();
            this.IsFlagged = true;
            this.IsBlank = false;
            this.IsNumber = false;
        }

        //public void SetDirty() => this.isDirty = true;

        private bool TestIsNumber(out int number)
        {
            number = 0;
            var @class = this.Class;
            var match = NumberRegex.Match(@class);

            if (!match.Success) return false;

            number = Convert.ToInt32(match.Groups[1].Value);
            return true;
        }
    }

    internal static class WebSquareExtensions
    {
        public static (int x, int y) ToIndex(this string[] idxString)
        {
            return (Convert.ToInt32(idxString[0]), Convert.ToInt32(idxString[1]));
        }
    }
}
