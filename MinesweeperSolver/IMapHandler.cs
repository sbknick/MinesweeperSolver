namespace MinesweeperSolver
{
    using System;
    using System.Collections.Generic;
    using global::MinesweeperSolver.Models;

    public interface IMapHandler
    {
        IEnumerable<ISquare> AllSquares { get; }

        void Click(ISquare square);
        void Clear(ISquare square);
        void Flag(ISquare square);
        void Flag(IEnumerable<ISquare> squares);
        ISquare GetSquare((int x, int y) index);
        ISquare GetRandomSquare();
        int GetNumberLeft(ISquare square);
        int GetNumberLeft(ISquare square, out IEnumerable<ISquare> adjacentBlanks);

        ISquare[] GetAdjacentBlanks(ISquare square);
        ISquare[] GetAdjacentFlags(ISquare square);
        ISquare[] GetAdjacentNumbers(ISquare square, Func<ISquareWrapper, bool> where = null);
        ISquare[] GetOrthoNumbers(ISquare square, Func<ISquareWrapper, bool> where = null);

        IEnumerable<ISquare> GetAdjacentSquares(ISquare square, Func<ISquare, bool> predicate = null);

        IEnumerable<ISquare> GetNumberSplat(ISquare square);
    }
}
