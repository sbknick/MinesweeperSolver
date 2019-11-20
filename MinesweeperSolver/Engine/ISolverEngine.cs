namespace MinesweeperSolver.Engine
{
    using System.Collections;

    public interface ISolverEngine
    {
        IEnumerable RunStep();
    }
}
