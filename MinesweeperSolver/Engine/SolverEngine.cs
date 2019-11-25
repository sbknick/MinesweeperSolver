namespace MinesweeperSolver.Engine
{
    using global::MinesweeperSolver.Models;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class SolverEngine : ISolverEngine
    {
        private IMapHandler MapHandler { get; }
        private IReadOnlyList<ILogicEngine> LogicEngines { get; }

        private readonly HashSet<ISquare> processedSquares = new HashSet<ISquare>();
        private readonly HashSet<ISquare> queuedSquares = new HashSet<ISquare>();

        public SolverEngine(IMapHandler mapHandler, IReadOnlyList<ILogicEngine> logicEngines)
        {
            this.MapHandler = mapHandler;
            this.LogicEngines = logicEngines;
        }

        public IEnumerable RunStep()
        {
            var anyStepSucceeded = false;

            // Is it more efficient to go each trick / step or each step / trick?
            foreach (var logicEngine in this.LogicEngines)
            {
                bool keepGoing;
                do
                {
                    keepGoing = logicEngine.TryDoStep(this.queuedSquares.ToArray(), out var processed, out var toProcess);
                    if (keepGoing) anyStepSucceeded = true;
                    this.processedSquares.AddRange(processed);
                    this.queuedSquares.RemoveAll(processed);
                    this.queuedSquares.AddRange(toProcess.Except(this.processedSquares));
                    yield return default;
                } while (keepGoing);
            }

            if (!anyStepSucceeded)
            {
                var allBlanks = this.MapHandler.AllSquares.Except(this.processedSquares).Except(this.queuedSquares);
                var randomGuess = allBlanks.First();
                randomGuess.Click();

                if (randomGuess.IsNumber)
                {
                    if (randomGuess.Number > 0)
                        this.queuedSquares.Add(randomGuess);
                    else
                        this.queuedSquares.AddRange(this.MapHandler.GetNumberSplat(randomGuess));
                }
                else
                {
                    
                }

                //foreach (var logicEngine in this.LogicEngines)
                //{
                //    logicEngine.StartFresh();
                //}
            }
        }
    }
}
