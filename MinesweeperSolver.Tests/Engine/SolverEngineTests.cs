namespace MinesweeperSolver.Tests.Engine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using global::MinesweeperSolver.Engine;
    using global::MinesweeperSolver.Models;
    using Moq;
    using NUnit.Framework;

    public class SolverEngineTests
    {
        private static readonly IEnumerable<int> nTimes = Enumerable.Range(1, 20);
        [Test, TestCaseSource(nameof(nTimes))]
        public void Should_CallSingleLogEngineNTimes(int n)
        {
            // Arrange
            IReadOnlyCollection<ISquare> emptyList = new List<ISquare>();

            var logicEngine = new Mock<ILogicEngine>();
            logicEngine
                .Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out emptyList))
                .ReturnsCount(i => i < n);

            var engine = new SolverEngine(Mock.Of<IMapHandler>(), new[] { logicEngine.Object });

            // Act
            foreach (var step in engine.RunStep()) { }

            // Assert
            logicEngine.Verify(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out emptyList), Times.Exactly(n));
        }

        private static readonly IEnumerable<(int, int, int)> nmoTimes =
            from i in Enumerable.Range(1, 3)
            from j in Enumerable.Range(1, 3)
            from k in Enumerable.Range(1, 3) 
            select (i, j, k);

        [Test, TestCaseSource(nameof(nmoTimes))]
        public void Should_CallMultipleLogEnginesNMOTimes((int, int, int) testCase)
        {
            // Arrange
            var (i, j, k) = testCase;
            IReadOnlyCollection<ISquare> emptyList = new List<ISquare>();

            Expression<Func<ILogicEngine, bool>> tryDoStep = x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out emptyList);

            var logicEngine1 = new Mock<ILogicEngine>();
            logicEngine1.Setup(tryDoStep).ReturnsCount(n => n < i);

            var logicEngine2 = new Mock<ILogicEngine>();
            logicEngine2.Setup(tryDoStep).ReturnsCount(n => n < j);

            var logicEngine3 = new Mock<ILogicEngine>();
            logicEngine3.Setup(tryDoStep).ReturnsCount(n => n < k);

            var engine = new SolverEngine(Mock.Of<IMapHandler>(), new []{ logicEngine1.Object, logicEngine2.Object, logicEngine3.Object });

            // Act
            foreach (var step in engine.RunStep()) { }

            // Assert
            logicEngine1.Verify(tryDoStep, Times.Exactly(i));
            logicEngine2.Verify(tryDoStep, Times.Exactly(j));
            logicEngine3.Verify(tryDoStep, Times.Exactly(k));
        }

        [Test]
        public void Should_QueueSquaresProperly()
        {
            var resultsList = new List<(List<ISquare> input, List<ISquare> processed, List<ISquare> toProcess)>();
            var captureArgs = new InvocationAction(a => resultsList.Add((
                ((IEnumerable<ISquare>)a.Arguments[0]).ToList(),
                ((IReadOnlyCollection<ISquare>)a.Arguments[1]).ToList(),
                ((IReadOnlyCollection<ISquare>)a.Arguments[2]).ToList()
            )));

            // Arrange
            IReadOnlyCollection<ISquare> emptyList = new List<ISquare>();

            var square1 = Mock.Of<ISquare>();
            IReadOnlyCollection<ISquare> newSquares1 = new List<ISquare> { square1 };

            var square2 = Mock.Of<ISquare>();
            IReadOnlyCollection<ISquare> newSquares2 = new List<ISquare> { square2 };

            var square3 = Mock.Of<ISquare>();
            IReadOnlyCollection<ISquare> newSquares3 = new List<ISquare> { square3 };

            var square4 = Mock.Of<ISquare>();
            IReadOnlyCollection<ISquare> newSquares4 = new List<ISquare> { square1, square4};

            //var storeParamsDelegate = new Delegate(())

            var engine1 = new Mock<ILogicEngine>();
            engine1.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out newSquares1))
                .Returns(false) // one step per
                .Callback(captureArgs);

            var engine2 = new Mock<ILogicEngine>();
            engine2.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out newSquares1, out newSquares2))
                .Returns(false) // one step per
                .Callback(captureArgs);

            var engine3 = new Mock<ILogicEngine>();
            engine3.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out newSquares3))
                .Returns(false) // one step per
                .Callback(captureArgs);

            var engine4 = new Mock<ILogicEngine>();
            engine4.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out newSquares4))
                .Returns(false) // one step per
                .Callback(captureArgs);

            var engine5 = new Mock<ILogicEngine>();
            engine5.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out newSquares4))
                .Returns(false) // one step per
                .Callback(captureArgs);

            var engine6 = new Mock<ILogicEngine>();
            engine6.Setup(x => x.TryDoStep(It.IsAny<IEnumerable<ISquare>>(), out emptyList, out emptyList))
                .Returns(false) // one step per
                .Callback(captureArgs);

            // Act
            var solverEngine = new SolverEngine(Mock.Of<IMapHandler>(), new []
            {
                engine1.Object,
                engine2.Object,
                engine3.Object,
                engine4.Object,
                engine5.Object,
                engine6.Object,
            });
            
            foreach (var step in solverEngine.RunStep()) { }

            // Assert

            // Engine1
            Assert.IsEmpty(resultsList[0].input);
            Assert.IsEmpty(resultsList[0].processed);
            Assert.Contains(square1, resultsList[0].toProcess);
            Assert.AreEqual(1, resultsList[0].toProcess.Count);

            // Engine2
            Assert.Contains(square1, resultsList[1].input);
            Assert.AreEqual(1, resultsList[1].input.Count);
            Assert.Contains(square1, resultsList[1].processed);
            Assert.AreEqual(1, resultsList[1].processed.Count);
            Assert.Contains(square2, resultsList[1].toProcess);
            Assert.AreEqual(1, resultsList[1].toProcess.Count);
         
            // Engine3
            Assert.Contains(square2, resultsList[2].input);
            Assert.AreEqual(1, resultsList[2].input.Count);
            Assert.IsEmpty(resultsList[2].processed);
            Assert.Contains(square3, resultsList[2].toProcess);
            Assert.AreEqual(1, resultsList[2].toProcess.Count);
            
            // Engine4
            Assert.Contains(square2, resultsList[3].input);
            Assert.Contains(square3, resultsList[3].input);
            Assert.AreEqual(2, resultsList[3].input.Count);
            Assert.AreEqual(newSquares4, resultsList[3].toProcess);

            // Engine5 - ShouldNot_PassAPreviouslyProcessedSquare_ToInputParam
            Assert.IsFalse(resultsList[4].input.Contains(square1));
            Assert.AreEqual(new List<ISquare> { square2, square3, square4 }, resultsList[4].input);
            Assert.AreEqual(3, resultsList[4].input.Count);

            // Engine6 - ShouldNot_AddSameSquareToInputTwice
            Assert.AreEqual(new List<ISquare> { square2, square3, square4 }, resultsList[4].input);
            Assert.AreEqual(3, resultsList[5].input.Count);

            Assert.AreEqual(6, resultsList.Count);
        }
    }
}
