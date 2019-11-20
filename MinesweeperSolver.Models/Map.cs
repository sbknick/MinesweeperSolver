namespace MinesweeperSolver.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Map : IMap
    {
        public int XMin => 1;
        public int YMin => 1;
        public int XMax => this.SquaresGrid.Keys.Max(k => k.x);
        public int YMax => this.SquaresGrid.Keys.Max(k => k.y);

        public IReadOnlyDictionary<(int x, int y), ISquare> SquaresGrid { get; set; }
        public abstract IMineCounter MineCounter { get; }
    }

    public interface IMap
    {
        int XMin { get; }
        int XMax { get; }
        int YMin { get; }
        int YMax { get; }

        IReadOnlyDictionary<(int x, int y), ISquare> SquaresGrid { get; }

        IMineCounter MineCounter { get; }
    }
}
