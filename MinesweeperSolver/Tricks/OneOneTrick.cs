namespace MinesweeperSolver.Tricks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class OneOneTrick : ITrick
    {
        private ISquare[] blankAdjacent;
        private ISquare[][] neighborsBlanks;

        private IMapHandler MapHandler { get; }

        public OneOneTrick(IMapHandler mapHandler)
        {
            this.MapHandler = mapHandler;
        }

        public bool Predicate(ISquare testSquare)
        {
            if (this.MapHandler.GetNumberLeft(testSquare, out var adjacentBlanks) != 1) return false;

            this.blankAdjacent = adjacentBlanks.ToArray();

            if (this.blankAdjacent.Length != 2) return false;

            IEnumerable<ISquare> blanks = null;

            this.neighborsBlanks = (
                    from adjNum in this.MapHandler.GetAdjacentNumbers(testSquare)
                    where this.MapHandler.GetNumberLeft(adjNum, out blanks) == 1
                          && blanks.Intersect(this.blankAdjacent).Count() == 2
                    select blanks.ToArray())
                .ToArray();

            return this.neighborsBlanks.Length > 0;
        }

        public void DoTheThing(out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> newNumbersToProcess)
        {
            processedSquares = Array.Empty<ISquare>();
            var toProcess = new List<ISquare>();

            foreach (var neighborBlanks in this.neighborsBlanks)
            {
                var toClick = neighborBlanks.Except(this.blankAdjacent);
                foreach (var sq in toClick)
                {
                    sq.Click();
                    toProcess.Add(sq);
                }
            }

            newNumbersToProcess = toProcess;
        }
    }
}
