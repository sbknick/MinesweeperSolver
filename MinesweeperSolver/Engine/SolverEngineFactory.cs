//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace MinesweeperSolver.Engine
//{
//    public class SolverEngineFactory
//    {
//        private IMapHandler MapHandler { get; }
//        private List<ILogicEngine> logicEngines = new List<ILogicEngine>();

//        public SolverEngineFactory(IMapHandler mapHandler)
//        {
//            this.MapHandler = mapHandler;
//        }

//        public SolverEngineFactory UseLogicEngine(ILogicEngine logicEngine)
//        {
//            this.logicEngines.Add(logicEngine);
//            return this;
//        }

//        public ISolverEngine Build()
//        {
//            return new SolverEngine(this.logicEngines);
//        }
//    }
//}
