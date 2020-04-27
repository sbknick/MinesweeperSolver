namespace MinesweeperSolver
{
    using global::MinesweeperSolver.Engine;
    using global::MinesweeperSolver.Models;
    using OpenQA.Selenium;

    public class MinesweeperSolver : IMinesweeperSolver
    {
        private readonly IMap _map;
        private ISolverEngine SolverEngine { get; set; }

        public MinesweeperSolver(IMap map)
        {
            this._map = map;
        }

        public Screenshot Run()
        {
            this.SolverEngine = this.Build();

            while (this._map.MineCounter.Count() > 0)
            {
                foreach (var step in this.SolverEngine.RunStep()) { }
            }

            return null;
        }

        private SolverEngine Build()
        {
            var mapHandler = new MapHandler(this._map);

            return new SolverEngine(mapHandler, new ILogicEngine[]
            {
                //new FullLogicEngine(mapHandler),
                new SimpleEngine(mapHandler),
                new TrickEngine(mapHandler),
            });
        }
    }
}

//namespace MinesweeperSolver
//{
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading.Tasks;
//    using global::MinesweeperSolver.Engine;
//    using global::MinesweeperSolver.WebDriver;
//    using OpenQA.Selenium;

//    public class MinesweeperSolver : IMinesweeperSolver
//    {
//        private IMinesweeperDriver MinesweeperDriver { get; }
//        //private ITrickEngine TrickEngine { get; }

//        private HashSet<ISquarezzzz> NumbersToProcess { get; } = new HashSet<ISquarezzzz>();

//        private HashSet<ISquarezzzz> SquaresAccountedFor { get; } = new HashSet<ISquarezzzz>();

//        public MinesweeperSolver(IMinesweeperDriver minesweeperDriver)
//        {
//            this.MinesweeperDriver = minesweeperDriver;
//            //this.TrickEngine = trickEngine ?? new TrickEngine(null);
//        }

//        public async Task<Screenshot> Run()
//        {
//            this.MinesweeperDriver.Initialize();

//            //while (this.MinesweeperDriver.MineCounter.Count() > 0)
//            //{
//                this.DoStep();
//                this.DoStep();
//                this.DoStep();
//                //}

//            return null;
//        }

//        private void DoStep()
//        {
//            if (this.NumbersToProcess.Count == 0)
//            {
//                this.DoRandom();
//            }

//            while (this.TryDoObvious(out var processedSquares, out var newSquaresToProcess))
//            {
//                newSquaresToProcess = newSquaresToProcess.Except(this.SquaresAccountedFor).ToList();

//                this.SquaresAccountedFor.AddRange(processedSquares);
//                this.NumbersToProcess.RemoveAll(processedSquares);

//                var zeroSquare = newSquaresToProcess.FirstOrDefault(sq => sq.Number == 0);
//                if (zeroSquare != null)
//                {
//                    this.EnqueueNewSplat(zeroSquare);
//                }
//                else
//                {
//                    this.NumbersToProcess.AddRange(newSquaresToProcess);
//                }
//            }

//            //if (this.TrickEngine.TryTricks(this.NumbersToProcess.ToArray(), out var newSquares))
//            //{

//            //}
//        }

//        private void DoRandom()
//        {
//            var square = this.MinesweeperDriver.GetRandomSquare();

//            square.Click();

//            this.EnqueueNewSplat(square);
//        }

//        private bool TryDoObvious(out List<ISquarezzzz> processedSquares, out List<ISquarezzzz> newSquaresToProcess)
//        {
//            processedSquares = new List<ISquarezzzz>();
//            newSquaresToProcess = new List<ISquarezzzz>();

//            foreach (var square in this.NumbersToProcess.ToArray())
//            {
//                var blanks = square.GetAdjacentBlanks();
//                var flags = square.GetAdjacentFlags();

//                var numLeft = square.Number - flags.Length;

//                if (numLeft > 0 && numLeft == blanks.Length)
//                {
//                    blanks.FlagEach();
//                    processedSquares.AddRange(blanks);
//                    numLeft = 0;
//                }

//                if (numLeft == 0)
//                {
//                    if (blanks.Length > 0)
//                    {
//                        square.ClearNearby();
//                        newSquaresToProcess.AddRange(square.GetAdjacentNumbers());
//                    }
//                    processedSquares.Add(square);
//                }
//            }

//            return processedSquares.Count + newSquaresToProcess.Count > 0;
//        }

//        private void EnqueueNewSplat(ISquarezzzz square)
//        {
//            var splat = square.GetSplat();

//            foreach (var sq in splat)
//            {
//                if (sq.Number == 0)
//                    this.SquaresAccountedFor.Add(sq);
//                else if (!this.SquaresAccountedFor.Contains(sq))
//                    this.NumbersToProcess.Add(sq);
//            }
//        }
//    }
//}
