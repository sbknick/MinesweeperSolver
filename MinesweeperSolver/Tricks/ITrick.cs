namespace MinesweeperSolver.Tricks
{
    using System.Collections.Generic;
    using global::MinesweeperSolver.Models;

    public interface ITrick
    {
        bool Predicate(ISquare testSquare);
        void DoTheThing(out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> newNumbersToProcess);
    }
}
