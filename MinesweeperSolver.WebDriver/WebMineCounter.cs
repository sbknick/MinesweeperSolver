namespace MinesweeperSolver.WebDriver
{
    using OpenQA.Selenium;
    using System;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    internal class WebMineCounter : IMineCounter
    {
        private IWebElement HundredsElement { get; }
        private IWebElement TensElement { get; }
        private IWebElement OnesElement { get; }

        public WebMineCounter(IWebDriver webDriver)
        {
            this.HundredsElement = webDriver.FindElement(By.Id("mines_hundreds"));
            this.TensElement = webDriver.FindElement(By.Id("mines_tens"));
            this.OnesElement = webDriver.FindElement(By.Id("mines_ones"));
        }

        public int Count()
        {
            var hundreds = Convert.ToInt32(this.HundredsElement.GetAttribute("class").Last());
            var tens = Convert.ToInt32(this.TensElement.GetAttribute("class").Last());
            var ones = Convert.ToInt32(this.OnesElement.GetAttribute("class").Last());

            return ones + tens * 10 + hundreds * 100;
        }
    }
}
