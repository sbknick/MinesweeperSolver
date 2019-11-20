namespace MinesweeperSolver.Tests.Engine
{
    using System.Collections.Generic;
    using System.Linq;
    using global::MinesweeperSolver.Engine;
    using global::MinesweeperSolver.Models;
    using global::MinesweeperSolver.Tricks;
    using Moq;
    using NUnit.Framework;

    public class TrickEngineTests
    {
        [Test]
        public void Should_CallTrickPredicate_OnEachSquare()
        {
            // Arrange
            var testSquares = new[]
            {
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
            };

            var testTricks = new[]
            {
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
            };
            var trickEngine = new TrickEngine(Mock.Of<IMapHandler>(), testTricks);

            // Act
            var result = trickEngine.TryDoStep(testSquares, out _, out _);

            // Assert
            Assert.IsFalse(result);
            foreach (var trick in testTricks.Select(Mock.Get))
            {
                trick.Verify(t => t.Predicate(It.IsAny<ISquare>()), Times.Exactly(testSquares.Length));
                trick.VerifyNoOtherCalls();
            }
        }

        [Test]
        public void Should_CallTrickDoTheThing_OnSquaresWherePredicateIsTrue()
        {
            // Arrange
            var emptySquareList = new ISquare[0] as IReadOnlyCollection<ISquare>;

            var testSquares = new[]
            {
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
            };

            var testTricks = new[]
            {
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
            };

            void SetupTruePredicateFor(int i)
            {
                var trickMock = Mock.Get(testTricks[i]);
                trickMock.Setup(t => t.Predicate(testSquares[i])).Returns(true);
                trickMock.Setup(t => t.DoTheThing(out emptySquareList, out emptySquareList));
            }
            SetupTruePredicateFor(0);
            SetupTruePredicateFor(1);
            SetupTruePredicateFor(2);

            var trickEngine = new TrickEngine(Mock.Of<IMapHandler>(), testTricks);

            // Act
            var result = trickEngine.TryDoStep(testSquares, out var processedSquares, out var newSquaresToProcess);

            // Assert
            Assert.IsFalse(result); // No squares are returned as either processed or newToProcess
            Assert.IsEmpty(processedSquares);
            Assert.IsEmpty(newSquaresToProcess);
            foreach (var trick in testTricks.Select(Mock.Get))
            {
                trick.Verify(t => t.Predicate(It.IsAny<ISquare>()), Times.Exactly(testSquares.Length));
                trick.Verify(t => t.DoTheThing(out emptySquareList, out emptySquareList), Times.Once);
                trick.VerifyNoOtherCalls();
            }
        }

        [Test]
        public void Should_ReturnTrue_WhenASquareHasBeenReturnedAsProcessed()
        {
            // Arrange
            var testSquares = new[]
            {
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
            };

            var testTricks = new[]
            {
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
            };

            var emptySquareList = new ISquare[0] as IReadOnlyCollection<ISquare>;
            var processedSquareList = new[]
            {
                testSquares[0],
            } as IReadOnlyCollection<ISquare>;

            var trickMock = Mock.Get(testTricks[0]);
            trickMock.Setup(t => t.Predicate(testSquares[0])).Returns(true);
            trickMock.Setup(t => t.DoTheThing(out processedSquareList, out emptySquareList));

            var trickEngine = new TrickEngine(Mock.Of<IMapHandler>(), testTricks);

            // Act
            var result = trickEngine.TryDoStep(testSquares, out var processedSquares, out var newSquaresToProcess);

            // Assert
            Assert.IsTrue(result); // No squares are returned as either processed or newToProcess
            Assert.IsTrue(processedSquares.Count == 1 && processedSquares.First() == processedSquareList.Single());
            Assert.IsEmpty(newSquaresToProcess);
        }

        [Test]
        public void Should_ReturnTrue_WhenASquareHasBeenReturnedAsNewToProcess()
        {
            // Arrange
            var testSquares = new[]
            {
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
                Mock.Of<ISquare>(),
            };

            var testTricks = new[]
            {
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
                Mock.Of<ITrick>(),
            };

            var emptySquareList = new ISquare[0] as IReadOnlyCollection<ISquare>;
            var newToProcessSquareList = new[]
            {
                testSquares[0],
            } as IReadOnlyCollection<ISquare>;

            var trickMock = Mock.Get(testTricks[0]);
            trickMock.Setup(t => t.Predicate(testSquares[0])).Returns(true);
            trickMock.Setup(t => t.DoTheThing(out emptySquareList, out newToProcessSquareList));

            var trickEngine = new TrickEngine(Mock.Of<IMapHandler>(), testTricks);

            // Act
            var result = trickEngine.TryDoStep(testSquares, out var processedSquares, out var newSquaresToProcess);

            // Assert
            Assert.IsTrue(result); // No squares are returned as either processed or newToProcess
            Assert.IsEmpty(processedSquares);
            Assert.IsTrue(newSquaresToProcess.Count == 1 && newSquaresToProcess.First() == newToProcessSquareList.Single());
        }
    }
}
