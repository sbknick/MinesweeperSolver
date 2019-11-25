namespace MinesweeperSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class MapHandler : IMapHandler
    {
        private IMap Map { get; }

        public MapHandler(IMap map)
        {
            this.Map = map;
        }

        public IEnumerable<ISquare> AllSquares => this.Map.SquaresGrid.Values;

        public void Click(ISquare square) => square.Click();

        public void Clear(ISquare square) => square.Clear();

        public void Flag(ISquare square) => square.Flag();
        public void Flag(IEnumerable<ISquare> squares)
        {
            foreach (var sq in squares) sq.Flag();
        }

        public ISquare GetSquare((int x, int y) index) =>
            this.Map.SquaresGrid.TryGetValue(index, out var square) ? square : null;

        public ISquare GetRandomSquare()
        {
            var random = new Random();
            var idx = (random.Next(this.Map.XMax) + 1, random.Next(this.Map.YMax) + 1);
            return this.GetSquare(idx);
        }

        public int GetNumberLeft(ISquare square) => new SquareWrapper(this, square).NumberLeft;

        public int GetNumberLeft(ISquare square, out IEnumerable<ISquare> adjacentBlanks)
        {
            var wrapper = new SquareWrapper(this, square);
            adjacentBlanks = wrapper.Blanks;
            return wrapper.NumberLeft;
        }

        public ISquare[] GetAdjacentBlanks(ISquare square) =>
            this.GetAdjacentSquares(square, sq => sq.IsBlank).ToArray();

        public ISquare[] GetAdjacentFlags(ISquare square) =>
            this.GetAdjacentSquares(square, sq => sq.IsFlagged).ToArray();

        public ISquare[] GetAdjacentNumbers(ISquare square, Func<ISquareWrapper, bool> where = null) =>
            this.GetAdjacentSquares(square, sq => sq.IsNumber && (where == null || where(new SquareWrapper(this, sq)))).ToArray();

        public ISquare[] GetOrthoNumbers(ISquare square, Func<ISquareWrapper, bool> where = null) =>
            this.GetOrthoSquares(square, sq => sq.IsNumber && (where == null || where(new SquareWrapper(this, sq)))).ToArray();

        public IEnumerable<ISquare> GetAdjacentSquares(ISquare square, Func<ISquare, bool> predicate = null)
        {
            for (var x = square.Index.x - 1; x <= square.Index.x + 1; x++)
                for (var y = square.Index.y - 1; y <= square.Index.y + 1; y++)
                {
                    if (x < this.Map.XMin || y < this.Map.YMin || x > this.Map.XMax || y > this.Map.YMax) continue;
                    if (x == square.Index.x && y == square.Index.y) continue;

                    var sq = this.Map.SquaresGrid[(x, y)];
                    if (predicate == null || predicate(sq))
                        yield return sq;
                }
        }

        public IEnumerable<ISquare> GetNumberSplat(ISquare square)
        {
            var toTest = new HashSet<ISquare> { square };
            var splat = new HashSet<ISquare> { square };

            while (toTest.Count > 0)
            {
                var sq = toTest.Pop();

                var adj = this.GetAdjacentNumbers(sq, sw => !splat.Contains(sw.Square));
                toTest.AddRange(adj.Where(s => s.Number == 0));
                splat.AddRange(adj);
            }

            return splat;
        }

        private IEnumerable<ISquare> GetOrthoSquares(ISquare square, Func<ISquare, bool> predicate)
        {
            var results = new List<ISquare>();

            var (x, y) = square.Index;

            if (x != this.Map.XMin) Square((x - 1, y));
            if (x != this.Map.XMax) Square((x + 1, y));
            if (y != this.Map.YMin) Square((x, y - 1));
            if (y != this.Map.YMax) Square((x, y + 1));

            return results;

            void Square((int x, int y) idx)
            {
                if (this.Map.SquaresGrid.TryGetValue(idx, out var sq) && predicate(sq))
                {
                    results.Add(sq);
                }
            }
        }
    }

    public interface ISquareWrapper
    {
        ISquare Square { get; }
        int NumberLeft { get; }
    }

    public struct SquareWrapper : ISquareWrapper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2933:Fields that are only assigned in the constructor should be \"readonly\"", Justification = "Actually is assigned, just bad parsing of new syntax: ??=")]
        private ISquare[] _adjacentSquares;
        private ISquare[] AdjacentSquares => this._adjacentSquares ??= this.Adjacent();

        private IMapHandler MapHandler { get; }
        public ISquare Square { get; }

        public IEnumerable<ISquare> Blanks => this.AdjacentSquares.Where(sq => sq.IsBlank);
        public int NumberLeft => this.Square.Number - this.AdjacentSquares.Count(sq => sq.IsFlagged);

        public SquareWrapper(IMapHandler mapHandler, ISquare square)
        {
            this.MapHandler = mapHandler;
            this.Square = square;
            this._adjacentSquares = null;
        }
     
        private ISquare[] Adjacent() => this.MapHandler.GetAdjacentSquares(this.Square).ToArray();
    }
}
