namespace MinesweeperSolver.WebDriver
{
    using MinesweeperSolver.Models;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using System;
    using System.Linq;

    public class WebMap : Map, IDisposable
    {
        private IWebDriver Driver { get; }

        public override IMineCounter MineCounter { get; }

        public WebMap()
        {
            this.Driver = new ChromeDriver
            {
                Url = "http://minesweeperonline.com/#200-night",
            };
            this.Driver.Manage().Window.FullScreen();

            var squareElements = this.Driver.FindElements(By.ClassName("square"));

            this.SquaresGrid =
                squareElements
                    .Where(el => el.Displayed)
                    .Select(el => (ISquare)new WebSquare(this.Driver, el))
                    .ToDictionary(x => x.Index);

            this.MineCounter = new WebMineCounter(this.Driver);
        }

        public void Dispose() => this.Driver?.Dispose();
    }
}
