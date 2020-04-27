namespace MinesweeperSolver.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class FullLogicEngine : ILogicEngine
    {
        private readonly (int x, int y) startIndex;

        private IMapHandler MapHandler { get; }

        private bool FirstPass { get; set; } = true;

        public FullLogicEngine(IMapHandler mapHandler, (int x, int y) startIndex = default)
        {
            this.MapHandler = mapHandler;
            this.startIndex = startIndex;
        }

        public bool TryDoStep(IEnumerable<ISquare> squaresToTest, out IReadOnlyCollection<ISquare> processedSquares,
            out IReadOnlyCollection<ISquare> squaresToProcess)
        {
            var processed = new HashSet<ISquare>();
            var toProcess = new HashSet<ISquare>();

            processedSquares = processed;
            squaresToProcess = toProcess;

            try
            {
                ExclusiveBits.Cleanup();

                var toTest = squaresToTest?.ToArray();

                if (toTest?.Any() == true)
                    return this.DoPass(toTest, ref processed, ref toProcess);

                if (!this.FirstPass)
                    return false;

                this.FirstPass = false;
                return this.DoStartClick(ref processed, ref toProcess);
            }
            finally
            {
                ExclusiveBits.Clear(processedSquares);
            }
        }

        public void StartFresh()
        {
            throw new NotImplementedException();
        }

        private bool DoStartClick(ref HashSet<ISquare> processed, ref HashSet<ISquare> toProcess)
        {
            var square = this.startIndex != default
                ? this.MapHandler.GetSquare(this.startIndex)
                : this.MapHandler.GetRandomSquare();

            this.MapHandler.Click(square);
            if (square.IsNumber && square.Number == 0)
            {
                var splat = this.MapHandler.GetNumberSplat(square).ToArray();
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
                    continue;
                }

                var xor = ExclusiveBits.Get(blanks).ToArray();

                foreach (var (mineCount, squares) in xor)
                {
                    if (blanks.Intersect(squares).Count() == squares.Length)
                    {
                        numLeft -= mineCount;
                        blanks = blanks.Except(squares).ToArray();
                    }
                }

                if (numLeft == blanks.Length)
                {
                    this.MapHandler.Flag(blanks);
                    processed.AddRange(blanks);
                    processed.Add(testSquare);
                    continue;
                }

                if (numLeft == 1)
                    ExclusiveBits.Set(1, blanks);
                else if (xor.Any() && numLeft == 0)
                {
                    this.MapHandler.Click(blanks);
                    ExclusiveBits.Clear(blanks);
                    processed.AddRange(blanks);
                }
            }

            return processed.Count + toProcess.Count > 0;
        }

        private static class ExclusiveBits
        {
            private static readonly Dictionary<ISquare, IList<(int mineCount, ISquare[] squares)>> exclusiveSquares = new Dictionary<ISquare, IList<(int mineCount, ISquare[] squares)>>();

            public static IEnumerable<(int mineCount, ISquare[] squares)> Get(ISquare[] toFind)
            {
                var resultList = new List<(int, ISquare[])>();

                foreach (var square in toFind)
                {
                    if (exclusiveSquares.TryGetValue(square, out var sqs))
                    {
                        resultList.AddRange(sqs);
                    }
                }
                //return toFind.SelectMany(s => exclusiveSquares.TryGetValue()[s]).Distinct();

                return resultList;
            }

            public static void Set(int mineCount, ISquare[] squares)
            {
                var value = (mineCount, squares);

                foreach (var square in squares)
                {
                    if (exclusiveSquares.TryGetValue(square, out var sqs))
                    {
                        if (sqs.Any(q => q.mineCount == mineCount && q.squares.SequenceEqual(squares)))
                            continue;
                    }
                    else
                    {
                        exclusiveSquares.Add(square, new List<(int mineCount, ISquare[] squares)>());
                    }

                    exclusiveSquares[square].Add(value);
                }
            }

            public static void Clear(IEnumerable<ISquare> squares)
            {
                foreach (var square in squares)
                {
                    if (!exclusiveSquares.TryGetValue(square, out var sqs)) continue;
                    if (!exclusiveSquares.Remove(square)) continue;
                    Clear(sqs.SelectMany(x => x.squares));
                }
            }

            public static void Cleanup()
            {
                var squares = exclusiveSquares.Values.SelectMany(tup => tup).SelectMany(x => x.squares).Where(sq => !sq.IsBlank);

                Clear(squares);
            }
        }
    }
}
