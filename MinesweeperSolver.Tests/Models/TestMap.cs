using System;
using System.Collections.Generic;
using System.Text;

namespace MinesweeperSolver.Tests.Models
{
    using System.Linq;
    using global::MinesweeperSolver.Models;

    public class TestMap : Map
    {
        private IEnumerable<TestSquare> Squares => this.SquaresGrid.Values.Cast<TestSquare>();

        public IMapHandler MapHandler { get; private set; }

        public (int x, int y) StartIndex { get; private set; }

        public IEnumerable<ISquare> TriggerSquares => this.Squares.Where(sq => sq.IsTrigger);

        public IEnumerable<ISquare> ExpectedBlank => this.Squares.Where(sq => sq.Expect.IsBlank());
        public IEnumerable<ISquare> ExpectedFlagged => this.Squares.Where(sq => sq.Expect.IsFlagged());
        public IEnumerable<ISquare> ExpectedProcessed => this.Squares.Where(sq => sq.Expect.IsProcessed());
        public IEnumerable<ISquare> ExpectedToProcess => this.Squares.Where(sq => sq.Expect.IsToProcess());

        public static TestMap CreateWithHandler(TestSquare[] squares, (int x, int y)? startIndex = null)
        {
            var testMap = new TestMap();
            var mapHandler = testMap.MapHandler = new MapHandler(testMap);
            testMap.StartIndex = startIndex ?? (1, 1);

            foreach (var sq in squares) sq.SetMapHandler(mapHandler);
            testMap.SquaresGrid = squares.Cast<ISquare>().ToDictionary(x => x.Index);

            return testMap;
        }

        public override IMineCounter MineCounter { get; }
    }

}
