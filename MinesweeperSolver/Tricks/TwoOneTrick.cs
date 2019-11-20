namespace MinesweeperSolver.Tricks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class TwoOneTrick : ITrick
    {
        private ISquare square;
        private ISquare[] orthoNumbers;

        private ISquare[] myBlanks;

        private IMapHandler MapHandler { get; }

        public TwoOneTrick(IMapHandler mapHandler)
        {
            this.MapHandler = mapHandler;
        }

        public bool Predicate(ISquare testSquare)
        {
            this.square = testSquare;
            if (this.MapHandler.GetNumberLeft(testSquare) != 2) return false;

            this.myBlanks = this.MapHandler.GetAdjacentBlanks(testSquare);
            if (this.myBlanks.Length != 3) return false;

            this.orthoNumbers = this.MapHandler.GetOrthoNumbers(testSquare, n => n.NumberLeft == 1);
            return this.orthoNumbers.Length > 0;
        }

        public void DoTheThing(out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> newNumbersToProcess)
        {
            newNumbersToProcess = Array.Empty<ISquare>();
            var flaggedSquares = new List<ISquare>();
            processedSquares = flaggedSquares;

            foreach (var neighbor in this.orthoNumbers)
            {
                var neighborBlanks = this.MapHandler.GetAdjacentBlanks(neighbor);
                var toFlag = neighborBlanks.Except(this.myBlanks).Single();
                toFlag.Flag();
                flaggedSquares.Add(toFlag);
            }

            //var flaggedSquares = new List<ISquare>();

            //foreach (var neighbor in this.orthoNumbers)
            //{
            //    Direction d1_1, d1_2, d2;
            //    if (neighbor.Index == this.square.Index.Up(1))
            //    {
            //        d2 = Direction.Up;
            //        d1_1 = Direction.Left;
            //        d1_2 = Direction.Right;
            //    }
            //    else if (neighbor.Index == this.square.Index.Down(1))
            //    {
            //        d2 = Direction.Down;
            //        d1_1 = Direction.Left;
            //        d1_2 = Direction.Right;
            //    }
            //    else if (neighbor.Index == (this.square.Index.x - 1, this.square.Index.y))
            //    {
            //        d2 = Direction.Left;
            //        d1_1 = Direction.Up;
            //        d1_2 = Direction.Down;
            //    }
            //    else if (neighbor.Index == (this.square.Index.x + 1, this.square.Index.y))
            //    {
            //        d2 = Direction.Right;
            //        d1_1 = Direction.Up;
            //        d1_2 = Direction.Down;
            //    }
            //    else
            //        throw new NotSupportedException();

            //    if (this.TryGetSquareToFlag(neighbor, d1_1, d2, out var squareToFlag) ||
            //        this.TryGetSquareToFlag(neighbor, d1_2, d2, out squareToFlag))
            //    {
            //        this.MapHandler.Flag(squareToFlag);
            //        flaggedSquares.Add(squareToFlag);
            //    }
            //}

            //processedSquares = flaggedSquares;
            //newNumbersToProcess = Array.Empty<ISquare>();
        }

        private enum Direction
        {
            Up, Down, Left, Right,
        }

        private bool TryGetSquareToFlag(ISquare neighbor, Direction direction1, Direction direction2, out ISquare squareToFlag)
        {
            squareToFlag = null;

            (int x, int y) blankCheckIdx1 = GetNext(this.square.Index, direction1),
                           blankCheckIdx2 = GetNext(blankCheckIdx1, direction2);

            var tempSquare = this.MapHandler.GetSquare(blankCheckIdx1);
            if (tempSquare == null || !tempSquare.IsBlank) return false;

            tempSquare = this.MapHandler.GetSquare(blankCheckIdx2);
            if (!tempSquare.IsBlank) return false;

            var neighborsBlanks = this.MapHandler.GetAdjacentBlanks(neighbor);
            if (neighborsBlanks.Length > 3) return false;

            squareToFlag = neighborsBlanks.Single(sq => sq.Index != blankCheckIdx1 && sq.Index != blankCheckIdx2);
            return true;
        }

        private (int x, int y) GetNext((int x, int y) index, Direction direction)
        {
            return direction switch
            {
                Direction.Up => index.Up(1),
                Direction.Down => index.Down(1),
                Direction.Left => index.Left(1),
                Direction.Right => index.Right(1),
                _ => throw new NotSupportedException()
            };

        }
    }
}
