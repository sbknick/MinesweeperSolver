namespace MinesweeperSolver.Engine
{
    using System.Collections.Generic;
    using global::MinesweeperSolver.Models;
    using global::MinesweeperSolver.Tricks;

    public class TrickEngine : ILogicEngine
    {
        private IReadOnlyList<ITrick> Tricks { get; }

        public TrickEngine(IMapHandler mapHandler, IReadOnlyList<ITrick> tricks = null)
        {
            this.Tricks = tricks ?? new ITrick[]
            {
                new OneNTrick(mapHandler),
                new OneOneTrick(mapHandler), 
            };
        }

        public bool TryDoStep(IEnumerable<ISquare> squaresToCheck, out IReadOnlyCollection<ISquare> processedSquares, out IReadOnlyCollection<ISquare> newSquaresToProcess)
        {
            var myProcessedSquares = new List<ISquare>();
            var myNewSquaresToProcess = new List<ISquare>();

            foreach (var square in squaresToCheck)
                foreach (var trick in this.Tricks)
                {
                    if (trick.Predicate(square))
                    {
                        trick.DoTheThing(out processedSquares, out newSquaresToProcess);
                        myProcessedSquares.AddRange(processedSquares);
                        myNewSquaresToProcess.AddRange(newSquaresToProcess);
                    }
                }

            processedSquares = myProcessedSquares;
            newSquaresToProcess = myNewSquaresToProcess;

            return processedSquares.Count + myNewSquaresToProcess.Count > 0;
        }

        public void StartFresh()
        {
        }
    }
}
