namespace MinesweeperSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using global::MinesweeperSolver.Models;

    public static class Extensions
    {
        public static T Pop<T>(this HashSet<T> set)
        {
            var item = set.First();
            set.Remove(item);
            return item;
        }

        public static void FlagRange(this IEnumerable<ISquare> squares)
        {
            foreach (var sq in squares)
                sq.Flag();
        }

        public static bool AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            var result = true;

            foreach (var item in items)
            {
                result &= set.Add(item);
            }

            return result;
        }

        public static bool RemoveAll<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            var result = true;

            foreach (var item in items)
            {
                result &= set.Remove(item);
            }

            return result;
        }

        public static (int x, int y) Up(this (int x, int y) src, int count)
            => (src.x, src.y - count);

        public static (int x, int y) Down(this (int x, int y) src, int count)
            => (src.x, src.y + count);

        public static (int x, int y) Left(this (int x, int y) src, int count)
            => (src.x - count, src.y);

        public static (int x, int y) Right(this (int x, int y) src, int count)
            => (src.x + count, src.y);
    }
}
