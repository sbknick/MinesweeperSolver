namespace MinesweeperSolver.Engine
{
    using System;
    using global::MinesweeperSolver.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class SimpleEngine : ILogicEngine
    {
        private readonly (int x, int y) startIndex;

        private IMapHandler MapHandler { get; }

        private bool FirstPass { get; set; } = true;
        private bool DoRandom { get; set; }

        public SimpleEngine(IMapHandler mapHandler, (int x, int y) startIndex = default)
        {
            this.MapHandler = mapHandler;
            this.startIndex = startIndex;
        }

        public bool TryDoStep(IEnumerable<ISquare> squaresToTest, out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> squaresToProcess)
        {
            var processed = new HashSet<ISquare>();
            var toProcess = new HashSet<ISquare>();

            processedSquares = processed;
            squaresToProcess = toProcess;

            if (this.DoRandom)
                return this.DoRandomClick(ref processed, ref toProcess);

            var toTest = squaresToTest?.ToArray();

            if (toTest?.Any() == true)
                return this.DoPass(toTest, ref processed, ref toProcess);

            if (!this.FirstPass)
                return false;

            this.FirstPass = false;
            return this.DoStartClick(ref processed, ref toProcess);
        }

        public void StartFresh()
        {
            this.DoRandom = true;
        }

        private bool DoStartClick(ref HashSet<ISquare> processed, ref HashSet<ISquare> toProcess)
        {
            var square = this.startIndex != default
                ? this.MapHandler.GetSquare(this.startIndex)
                : this.MapHandler.GetRandomSquare();

            this.MapHandler.Click(square);
            if (square.IsNumber && square.Number == 0)
            {
                var splat = this.MapHandler.GetAllNumbers(square).ToArray();
                foreach (var sq in splat)
                {
                    if (sq.Number == 0)
                        processed.Add(sq);
                    else
                        toProcess.Add(sq);
                }
                return true;
            }

            throw new ArgumentException();
        }

        private bool DoPass(ISquare[] squaresToTest, ref HashSet<ISquare> processed,
            ref HashSet<ISquare> toProcess)
        {
            foreach (var testSquare in squaresToTest)
            {
                var numLeft = this.MapHandler.GetNumberLeft(testSquare);

                if (numLeft == 0)
                {
                    this.MapHandler.Clear(testSquare);
                    var adjacentNumbers = this.MapHandler.GetAdjacentNumbers(testSquare);
                    toProcess.AddRange(adjacentNumbers.Where(n => !squaresToTest.Contains(n)));
                    processed.Add(testSquare);
                    continue;
                }

                var blanks = this.MapHandler.GetAdjacentBlanks(testSquare);
                if (numLeft == blanks.Length)
                {
                    this.MapHandler.Flag(blanks);
                    processed.AddRange(blanks);
                    processed.Add(testSquare);
                }
            }

            return processed.Count + toProcess.Count > 0;
        }

        private bool DoRandomClick(ref HashSet<ISquare> processed, ref HashSet<ISquare> toProcess)
        {
            this.DoRandom = false;

            //var allBlanks = this.MapHandler.AllSquares.Except(this.);

            return false;
        }
    }
}
