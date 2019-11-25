namespace MinesweeperSolver.Tests.Engine
{
    using global::MinesweeperSolver;
    using global::MinesweeperSolver.Engine;
    using global::MinesweeperSolver.Models;
    using global::MinesweeperSolver.Tests.Models;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using System.Collections.Generic;
    using System.Linq;

    public class SimpleEngineTests
    {
        [Test]
        public void TryDoStep_ShouldDoIndicatedStartIndexFirst()
        {
            // Arrange
            var startIndex = (4, 4);

            var square = new Mock<ISquare>();
            square.SetupGet(x => x.IsNumber).Returns(true);
            square.SetupGet(x => x.Number).Returns(0);

            var mapHandler = new Mock<IMapHandler>();
            mapHandler
                .Setup(x => x.GetSquare(startIndex))
                .Returns(square.Object);

            // Act
            var simpleEngine = new SimpleEngine(mapHandler.Object, startIndex);
            var result = simpleEngine.TryDoStep(null, out _, out _);

            // Assert
            Assert.IsTrue(result);
            mapHandler.Verify(x => x.GetSquare(startIndex), Times.Once);
            mapHandler.Verify(x => x.Click(square.Object), Times.Once);
            mapHandler.Verify(x => x.GetNumberSplat(square.Object), Times.Once);
            square.VerifyGet(x => x.IsNumber, Times.Once);
            square.VerifyGet(x => x.Number,  Times.Once);
            mapHandler.VerifyNoOtherCalls();
        }

        [Test]
        public void TryDoStep_ShouldDoRandomStartIndexFirst_IfNotSupplied()
        {
            // Arrange
            var mapHandler = new Mock<IMapHandler>();
            mapHandler
                .Setup(x => x.GetRandomSquare())
                .Returns(new TestSquare { IsNumber = true, Number = 0 });

            // Act
            var simpleEngine = new SimpleEngine(mapHandler.Object);
            var result = simpleEngine.TryDoStep(null, out _, out _);

            // Assert
            Assert.IsTrue(result);
            mapHandler.Verify(x => x.GetRandomSquare(), Times.Once);
            mapHandler.Verify(x => x.Click(It.IsAny<ISquare>()), Times.Once);
            mapHandler.Verify(x => x.GetNumberSplat(It.IsAny<ISquare>()), Times.Once);
            mapHandler.VerifyNoOtherCalls();
        }

        private static IEnumerable<TestMap> SimpleTestMaps()
        {

            /* +-+-+-+-+
             * |1|2|2|1|
             * |1|#|#|#|
             * |#|#|#|#|
             * +-+-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsNumber = true, Number = 1, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (2, 1), IsNumber = true, Number = 2, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (3, 1), IsNumber = true, Number = 2, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (4, 1), IsNumber = true, Number = 1, Expect = Expect.Processed, IsTrigger = true },

                new TestSquare { Index = (1, 2), IsNumber = true, Number = 1, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (2, 2), IsBlank = true, Expect = Expect.Processed | Expect.Flagged },
                new TestSquare { Index = (3, 2), IsBlank = true, Expect = Expect.Processed | Expect.Flagged },
                new TestSquare { Index = (4, 2), IsBlank = true, Expect = Expect.NewToProcess },

                new TestSquare { Index = (1, 3), IsBlank = true, Expect = Expect.NewToProcess },
                new TestSquare { Index = (2, 3), IsBlank = true, Expect = Expect.NewToProcess },
                new TestSquare { Index = (3, 3), IsBlank = true, Expect = Expect.Blank },
                new TestSquare { Index = (4, 3), IsBlank = true, Expect = Expect.Blank },
            });

            /* +-+-+-+
             * |9|3|#|
             * |9|#|#|
             * |#|2|#|
             * +-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsNumber = true, Number = 9, IsTrigger = true },
                new TestSquare { Index = (2, 1), IsNumber = true, Number = 3, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (3, 1), IsBlank = true, Expect = Expect.Processed | Expect.Flagged },

                new TestSquare { Index = (1, 2), IsNumber = true, Number = 9, IsTrigger = true },
                new TestSquare { Index = (2, 2), IsBlank = true, Expect = Expect.Processed | Expect.Flagged },
                new TestSquare { Index = (3, 2), IsBlank = true, Expect = Expect.Processed | Expect.Flagged },

                new TestSquare { Index = (1, 3), IsBlank = true, Expect = Expect.NewToProcess },
                new TestSquare { Index = (2, 3), IsNumber = true, Number = 2, Expect = Expect.Processed, IsTrigger = true },
                new TestSquare { Index = (3, 3), IsBlank = true, Expect = Expect.NewToProcess },
            });
        }

        [Test, TestCaseSource(nameof(SimpleTestMaps))]
        public void TryDoStep_ClearsAndFlagsExpectedSquares(TestMap testMap)
        {
            // Arrange
            var simpleEngine = new SimpleEngine(testMap.MapHandler);

            // Act
            var result = simpleEngine.TryDoStep(testMap.TriggerSquares, out var processedSquares, out var squaresToProcess);

            // Assert
            Assert.IsTrue(result);
            CustomAssert.AreEqual(testMap.ExpectedProcessed, processedSquares);
            CustomAssert.AreEqual(testMap.ExpectedToProcess, squaresToProcess);
            CustomAssert.AreFlagged(testMap.ExpectedFlagged);
        }

        private static IEnumerable<TestMap> MultiStepTestMaps()
        {
            /* +-+-+-+-+
             * |0|1|2|2|
             * |0|1|#|#|
             * |0|1|4|#|
             * |0|0|2|#|
             * +-+-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 1), IsBlank = true, Number = 1, Expect = Expect.Processed },
                new TestSquare { Index = (3, 1), IsBlank = true, Number = 2, Expect = Expect.Processed },
                new TestSquare { Index = (4, 1), IsBlank = true, Number = 2, Expect = Expect.Processed },

                new TestSquare { Index = (1, 2), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 2), IsBlank = true, Number = 1, Expect = Expect.Processed },
                new TestSquare { Index = (3, 2), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (4, 2), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 3), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 3), IsBlank = true, Number = 1, Expect = Expect.Processed },
                new TestSquare { Index = (3, 3), IsBlank = true, Number = 4, Expect = Expect.Processed },
                new TestSquare { Index = (4, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 4), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 4), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (3, 4), IsBlank = true, Number = 2, Expect = Expect.Processed },
                new TestSquare { Index = (4, 4), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
            });

            /* +-+-+-+-+
             * |0|0|1|#|
             * |2|2|3|2|
             * |#|#|2|#|
             * |#|3|2|1|
             * +-+-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (3, 1), IsBlank = true, Number = 1 },
                new TestSquare { Index = (4, 1), IsBlank = true, Expect = Expect.Blank },

                new TestSquare { Index = (1, 2), IsBlank = true, Number = 2, Expect = Expect.Processed  },
                new TestSquare { Index = (2, 2), IsBlank = true, Number = 2, Expect = Expect.Processed  },
                new TestSquare { Index = (3, 2), IsBlank = true, Number = 3 },
                new TestSquare { Index = (4, 2), IsBlank = true, Number = 2,Expect = Expect.Blank },

                new TestSquare { Index = (1, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (3, 3), IsBlank = true, Number = 2 },
                new TestSquare { Index = (4, 3), IsBlank = true, Expect = Expect.Blank },

                new TestSquare { Index = (1, 4), IsBlank = true, Expect = Expect.Blank },
                new TestSquare { Index = (2, 4), IsBlank = true, Number = 3, Expect = Expect.Blank },
                new TestSquare { Index = (3, 4), IsBlank = true, Number = 2, Expect = Expect.Blank },
                new TestSquare { Index = (4, 4), IsBlank = true, Number = 1, Expect = Expect.Blank },
            });

            /* +-+-+-+-+
             * |0|0|0|0|
             * |2|2|1|0|
             * |#|#|2|1|
             * |#|3|2|#|
             * +-+-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (2, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (3, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (4, 1), IsBlank = true, Number = 0, Expect = Expect.Processed },

                new TestSquare { Index = (1, 2), IsBlank = true, Number = 2, Expect = Expect.Processed },
                new TestSquare { Index = (2, 2), IsBlank = true, Number = 2, Expect = Expect.Processed },
                new TestSquare { Index = (3, 2), IsBlank = true, Number = 1, Expect = Expect.Processed },
                new TestSquare { Index = (4, 2), IsBlank = true, Number = 0, Expect = Expect.Processed },

                new TestSquare { Index = (1, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (3, 3), IsBlank = true, Number = 2 },
                new TestSquare { Index = (4, 3), IsBlank = true, Number = 1 },

                new TestSquare { Index = (1, 4), IsBlank = true, Expect = Expect.Blank },
                new TestSquare { Index = (2, 4), IsBlank = true, Number = 3, Expect = Expect.Blank },
                new TestSquare { Index = (3, 4), IsBlank = true, Number = 2, Expect = Expect.Blank },
                new TestSquare { Index = (4, 4), IsBlank = true, Expect = Expect.Blank },
            });

            /* +-+-+-+-+-+
             * |#|#|#|#|#|
             * |#|5|3|5|#|
             * |#|3|0|3|#|
             * |#|5|3|5|#|
             * |#|#|#|#|#|
             * +-+-+-+-+-+
             */
            yield return TestMap.CreateWithHandler(new[]
            {
                new TestSquare { Index = (1, 1), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 1), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (3, 1), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (4, 1), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (5, 1), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 2), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 2), IsBlank = true, Number = 5, Expect = Expect.Processed },
                new TestSquare { Index = (3, 2), IsBlank = true, Number = 3, Expect = Expect.Processed },
                new TestSquare { Index = (4, 2), IsBlank = true, Number = 5, Expect = Expect.Processed },
                new TestSquare { Index = (5, 2), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 3), IsBlank = true, Number = 3, Expect = Expect.Processed },
                new TestSquare { Index = (3, 3), IsBlank = true, Number = 0, Expect = Expect.Processed },
                new TestSquare { Index = (4, 3), IsBlank = true, Number = 3, Expect = Expect.Processed },
                new TestSquare { Index = (5, 3), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 4), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 4), IsBlank = true, Number = 5, Expect = Expect.Processed },
                new TestSquare { Index = (3, 4), IsBlank = true, Number = 3, Expect = Expect.Processed },
                new TestSquare { Index = (4, 4), IsBlank = true, Number = 5, Expect = Expect.Processed},
                new TestSquare { Index = (5, 4), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },

                new TestSquare { Index = (1, 5), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (2, 5), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (3, 5), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (4, 5), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
                new TestSquare { Index = (5, 5), IsBlank = true, Expect = Expect.Flagged | Expect.Processed },
            },
            startIndex: (3, 3));
        }

        [Test, TestCaseSource(nameof(MultiStepTestMaps))]
        public void MultipleSteps_AreProcessedCorrectly(TestMap testMap)
        {
            // Arrange
            var simpleEngine = new SimpleEngine(testMap.MapHandler, testMap.StartIndex);

            // Act
            var totalProcessed = new HashSet<ISquare>();
            var nextToProcess = testMap.TriggerSquares.ToArray();

            while (simpleEngine.TryDoStep(nextToProcess, out var processedSquares, out var squaresToProcess))
            {
                totalProcessed.AddRange(processedSquares);
                nextToProcess = nextToProcess.Union(squaresToProcess).Except(totalProcessed).ToArray();
            }

            // Assert
            CustomAssert.AreEqual(testMap.ExpectedProcessed, totalProcessed);
            CustomAssert.AreBlank(testMap.ExpectedBlank);
            CustomAssert.AreFlagged(testMap.ExpectedFlagged);
        }

        private struct CustomAssert
        {
            public static void AreEqual(IEnumerable<ISquare> expected, IEnumerable<ISquare> actual) =>
                Assert.AreEqual(
                    expected.OrderBy(sq => sq.Index).ToArray(),
                    actual.OrderBy(sq => sq.Index).ToArray());

            public static void AreBlank(IEnumerable<ISquare> expected) =>
                Assert.IsTrue(expected.All(sq => sq.IsBlank), FailureMessage("blank", expected));

            public static void AreFlagged(IEnumerable<ISquare> expected) =>
                Assert.IsTrue(expected.All(sq => sq.IsFlagged), FailureMessage("flagged", expected));

            private static string FailureMessage(string word, object toSerialize) =>
                $"Not all squares expected were {word}.\n{JsonConvert.SerializeObject(toSerialize)}";
        }
    }
}
