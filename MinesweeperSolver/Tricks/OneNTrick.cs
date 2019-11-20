namespace MinesweeperSolver.Tricks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class OneNTrick : ITrick
    {
        private ISquare[] blankAdjacent;
        private ISquare[][] neighborsBlanks;

        private IMapHandler MapHandler { get; }

        public OneNTrick(IMapHandler mapHandler)
        {
            this.MapHandler = mapHandler;
        }

        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public bool Predicate(ISquare testSquare)
        {
            if (this.MapHandler.GetNumberLeft(testSquare, out var adjacentBlanks) != 1) return false;

            this.blankAdjacent = adjacentBlanks.ToArray();

            IEnumerable<ISquare> blanks = null;

            this.neighborsBlanks = (
                from adjNum in this.MapHandler.GetAdjacentNumbers(testSquare)
                where this.MapHandler.GetNumberLeft(adjNum, out blanks) + 1 == blanks.Count()
                      && blanks.Intersect(this.blankAdjacent).Count() == 2
                select blanks.ToArray())
                .ToArray();

            return this.neighborsBlanks.Length > 0;
        }

        public void DoTheThing(out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> newNumbersToProcess)
        {
            newNumbersToProcess = Array.Empty<ISquare>();
            var processed = new List<ISquare>();

            foreach (var neighborBlanks in this.neighborsBlanks)
            {
                var toFlag = neighborBlanks.Except(this.blankAdjacent);
                foreach (var sq in toFlag)
                {
                    sq.Flag();
                    processed.Add(sq);
                }
            }

            processedSquares = processed;
        }
    }
}
