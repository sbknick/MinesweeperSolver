namespace MinesweeperSolver.Models
{
    public interface ISquare
    {
        (int x, int y) Index { get; }
        bool IsBlank { get; }
        bool IsFlagged { get; }
        bool IsNumber { get; }
        int Number { get; }

        void Clear();
        void Click();
        void Flag();
    }
}
