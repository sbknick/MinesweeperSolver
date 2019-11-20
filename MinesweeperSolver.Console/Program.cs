namespace MinesweeperSolver.Console
{
    using global::MinesweeperSolver.WebDriver;

    public static class Program
    {
        public static void Main()
        {
            using (var map = new WebMap())
            {
                var solver = new MinesweeperSolver(map);
                var screenshot = solver.Run();
            }


            //    var map = WebMap.Load();

            //    var mapHandler = new MapHandler();
            //    var solver = new SolverEngine(
            //        new SimpleEngine(),
            //        new TrickEngine(mapHandler));

            //    while (true)
            //    {
            //        foreach (var step in solver.RunStep()) { }
            //    }

            //    //var solver = new MinesweeperSolver(new MinesweeperDriver(new ChromeDriver()));

            //    //var result = await solver.Run();
        }
    }
}
