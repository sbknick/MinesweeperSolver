//namespace MinesweeperSolver.Tests.Tricks
//{
//    using global::MinesweeperSolver.Models;
//    using global::MinesweeperSolver.Tricks;
//    using NUnit.Framework;
//    using System.Collections.Generic;
//    using System.Linq;
//    using global::MinesweeperSolver.Tests.Models;

//    public class OneTwoTrickTests
//    {
//        public class TestCase
//        {
//            public Map Map { get; set; }
//            public (int x, int y) TriggerIndex { get; set; }
//            public (int x, int y) ExpectedFlaggedIndex { get; set; }
//        }

//        private static IEnumerable<TestCase> GoodTestCases()
//        {
//            /* To the Right cases */

//            /*   +-+-+-+
//             *   | | | |
//             *   |1|2|9|
//             *   |#|#|#|
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 1), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 1), IsNumber = true, Number = 0 },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                        new TestSquare { Index = (3, 3), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 2),
//                ExpectedFlaggedIndex = (3, 3),
//            };

//            /*   +-+-+-+
//             *   |#|#|#|
//             *   |1|2|9|
//             *   | | | |
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsBlank = true },
//                        new TestSquare { Index = (2, 1), IsBlank = true },
//                        new TestSquare { Index = (3, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 3), IsNumber = true, Number = 0 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 2),
//                ExpectedFlaggedIndex = (3, 1),
//            };

//            /*   +-+-+-+
//             *   |#|#|9|
//             *   |1|2|9|
//             *   | | |#|
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsBlank = true },
//                        new TestSquare { Index = (2, 1), IsBlank = true },
//                        new TestSquare { Index = (3, 1), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 3), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 2),
//                ExpectedFlaggedIndex = (3, 3),
//            };

//            /* To the Left cases */

//            /*   +-+-+-+
//             *   | | | |
//             *   |9|2|1|
//             *   |#|#|#|
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 1), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 1), IsNumber = true, Number = 0 },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 1 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                        new TestSquare { Index = (3, 3), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (3, 2),
//                ExpectedFlaggedIndex = (1, 3),
//            };

//            /*   +-+-+-+
//             *   |#|#|#|
//             *   |9|2|1|
//             *   | | | |
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsBlank = true },
//                        new TestSquare { Index = (2, 1), IsBlank = true },
//                        new TestSquare { Index = (3, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 1 },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 3), IsNumber = true, Number = 0 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (3, 2),
//                ExpectedFlaggedIndex = (1, 1),
//            };

//            /*   +-+-+-+
//             *   |9|#|#|
//             *   |9|2|1|
//             *   |#| | |
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 1), IsBlank = true },
//                        new TestSquare { Index = (3, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 1 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (3, 3), IsNumber = true, Number = 0 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (3, 2),
//                ExpectedFlaggedIndex = (1, 3),
//            };

//            /* Down cases */

//            /*   +-+-+-+
//             *   | |1|#|
//             *   | |2|#|
//             *   | |#| |
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 1), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (3, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsBlank = true },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                        new TestSquare { Index = (3, 3), IsNumber = true, Number = 0 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (2, 1),
//                ExpectedFlaggedIndex = (2, 3),
//            };

//            /*   +-+-+-+
//             *   |#|1| |
//             *   |#|2| |
//             *   | |#| |
//             *   +-+-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsBlank = true },
//                        new TestSquare { Index = (2, 1), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (3, 1), IsNumber = true, Number = 0 },

//                        new TestSquare { Index = (1, 2), IsBlank = true },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (3, 2), IsNumber = true, Number = 0 },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 0 },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                        new TestSquare { Index = (3, 3), IsNumber = true, Number = 0 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (2, 1),
//                ExpectedFlaggedIndex = (2, 3),
//            };

//            /* Up cases */

//            /*   +-+-+
//             *   |#|9|
//             *   |2|#|
//             *   |1|#|
//             *   |9|#|
//             *   +-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsBlank = true },
//                        new TestSquare { Index = (2, 1), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (2, 2), IsBlank = true },

//                        new TestSquare { Index = (1, 3), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 3), IsBlank = true },

//                        new TestSquare { Index = (1, 4), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 4), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 3),
//                ExpectedFlaggedIndex = (1, 1),
//            };

//            /*   +-+-+
//             *   |9|#|
//             *   |#|2|
//             *   |#|1|
//             *   |#|9|
//             *   +-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsBlank = true },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 1 },

//                        new TestSquare { Index = (1, 4), IsBlank = true },
//                        new TestSquare { Index = (2, 4), IsNumber = true, Number = 9 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (2, 3),
//                ExpectedFlaggedIndex = (2, 1),
//            };

//            /* Should also work when the 1 is a 1-left, not just a base 1... */

//            /*   +-+-+
//             *   |9|#|
//             *   |#|2|
//             *   |#|2|
//             *   |F|9|
//             *   +-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 9 },
//                        new TestSquare { Index = (2, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsBlank = true },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 2 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsNumber = true, Number = 2 },

//                        new TestSquare { Index = (1, 4), IsFlagged = true },
//                        new TestSquare { Index = (2, 4), IsNumber = true, Number = 9 },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (2, 3),
//                ExpectedFlaggedIndex = (2, 1),
//            };
//        }

//        private static IEnumerable<TestCase> BadTestCases()
//        {
//            /*   +-+-+
//             *   |1|#|
//             *   |2|9|
//             *   |#|#|
//             *   +-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (2, 2), IsNumber = true, Number = 9 },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 1),
//            };

//            /*   +-+-+
//             *   |1|#|
//             *   |2|9|
//             *   |#|#|
//             *   +-+-+
//             */
//            yield return new TestCase
//            {
//                Map = new Map
//                {
//                    SquaresGrid = new ISquare[]
//                    {
//                        new TestSquare { Index = (1, 1), IsNumber = true, Number = 1 },
//                        new TestSquare { Index = (2, 1), IsBlank = true },

//                        new TestSquare { Index = (1, 2), IsNumber = true, Number = 2 },
//                        new TestSquare { Index = (2, 2), IsBlank = true },

//                        new TestSquare { Index = (1, 3), IsBlank = true },
//                        new TestSquare { Index = (2, 3), IsBlank = true },
//                    }.ToDictionary(x => x.Index),
//                },
//                TriggerIndex = (1, 1),
//            };
//        }

//        [Test, TestCaseSource(nameof(GoodTestCases))]
//        public void Predicate_ShouldReturnTrueFalseCorrectly(TestCase testCase)
//        {
//            // Arrange
//            var mapHandler = new MapHandler(testCase.Map);
//            var trick = new OneTwoTrick(mapHandler);
            
//            // Act
//            IEnumerable<bool> Execute()
//            {
//                for (var x = testCase.Map.XMin; x <= testCase.Map.XMax; x++)
//                    for (var y = testCase.Map.YMin; y <= testCase.Map.YMax; y++)
//                        yield return trick.Predicate(mapHandler.GetSquare((x, y)));
//            }

//            var results = Execute().ToArray();
//            var trueResult = trick.Predicate(mapHandler.GetSquare(testCase.TriggerIndex));

//            // Assert
//            Assert.AreEqual((testCase.Map.XMax * testCase.Map.YMax) - 1, results.Count(r => !r));
//            Assert.AreEqual(1, results.Count(r => r));
//            Assert.IsTrue(trueResult);
//        }

//        [Test, TestCaseSource(nameof(GoodTestCases))]
//        public void DoTheThing_ShouldFlagTheCorrectSquares(TestCase testCase)
//        {
//            // Arrange
//            var mapHandler = new MapHandler(testCase.Map);
//            var trick = new OneTwoTrick(mapHandler);
//            trick.Predicate(mapHandler.GetSquare(testCase.TriggerIndex));

//            // Act
//            trick.DoTheThing(out var processedSquares, out var newNumbersToProcess);

//            // Assert
//            Assert.AreEqual(1, processedSquares.Count);
//            Assert.IsEmpty(newNumbersToProcess);
//            var expectedSquare = mapHandler.GetSquare(testCase.ExpectedFlaggedIndex);
//            Assert.IsTrue(expectedSquare.IsFlagged);
//            Assert.AreEqual(expectedSquare, processedSquares.Single());
//        }

//        [Test, TestCaseSource(nameof(BadTestCases))]
//        public void DoTheThing_ShouldNotFlagAnything_IfIsBad(TestCase testCase)
//        {
//            // Arrange
//            var mapHandler = new MapHandler(testCase.Map);
//            var trick = new OneTwoTrick(mapHandler);
//            trick.Predicate(mapHandler.GetSquare(testCase.TriggerIndex));

//            // Act
//            trick.DoTheThing(out var processedSquares, out var newNumbersToProcess);

//            // Assert
//            Assert.IsEmpty(processedSquares);
//            Assert.IsEmpty(newNumbersToProcess);
//        }
//    }
//}
