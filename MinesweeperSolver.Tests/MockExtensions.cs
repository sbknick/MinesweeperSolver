namespace MinesweeperSolver.Tests
{
    using System;
    using Moq.Language.Flow;

    public static class MockExtensions
    {
        public static void ReturnsCount<T1, T2>(this ISetup<T1, T2> setup, Func<int, T2> action)
        where T1 : class
        {
            var i = 1;
            setup.Returns(() => action(i))
                .Callback(() => i++);
        }
    }
}
