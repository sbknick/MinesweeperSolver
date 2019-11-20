namespace MinesweeperSolver.Engine
{
    using System.Collections.Generic;
    using global::MinesweeperSolver.Models;

    public interface ILogicEngine
    {
        /// <summary>
        /// This should return true if it would benefit this engine to run more than once.
        /// </summary>
        /// <param name="squaresToTest"></param>
        /// <param name="processedSquares"></param>
        /// <param name="squaresToProcess"></param>
        /// <returns></returns>
        bool TryDoStep(IEnumerable<ISquare> squaresToTest, out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> squaresToProcess);

        /// <summary>
        /// If needed, reset the state of the engine to allow further (usually random) progression.
        /// </summary>
        void StartFresh();
    }
}
