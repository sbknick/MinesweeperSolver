namespace MinesweeperSolver.Tests.Models
{
    using global::MinesweeperSolver.Models;
    using System;
    using Newtonsoft.Json;

    public class TestSquare : ISquare
    {
        private bool isBlank;
        private bool isFlagged;
        private bool isNumber;
        
        private IMapHandler MapHandler { get; set; }

        public (int x, int y) Index { get; set; }

        public bool IsBlank
        {
            get => this.isBlank;
            set
            {
                if (this.isFlagged || this.isNumber)
                    throw new NotSupportedException();
                this.isBlank = value;
            }
        }

        public bool IsFlagged
        {
            get => this.isFlagged;
            set
            {
                if (this.isNumber) throw new NotSupportedException();

                this.isBlank = !value;
                this.isFlagged = value;
            }
        }

        public bool IsNumber
        {
            get => this.isNumber;
            set
            {
                if (this.isFlagged) throw new NotSupportedException();

                this.isBlank = !value;
                this.isNumber = value;
            }
        }
        
        public int Number { get; set; }

        public Expect Expect { get; set; }

        public bool IsTrigger { get; set; }

        public void Clear()
        {
            var adj = this.MapHandler.GetAdjacentBlanks(this);
            foreach (var sq in adj) sq.Click();
        }

        public void Click()
        {
            if (this.IsNumber)
                return;

            this.IsNumber = true;
            if (this.Number == 0)
            {
                this.Clear();
            }
        }

        public void Flag() => this.IsFlagged = true;

        public void SetMapHandler(IMapHandler mapHandler) => this.MapHandler = mapHandler;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    [Flags]
    public enum Expect
    {
        // ReSharper disable once ShiftExpressionRealShiftCountIsZero
        Processed = 1 << 0,
        Flagged = 1 << 1,
        NewToProcess = 1 << 2,
        Blank = 1 << 3,
    }

    public static class ExpectExt
    {
        public static bool IsBlank(this Expect expect) => (expect & Expect.Blank) == Expect.Blank;
        public static bool IsFlagged(this Expect expect) => (expect & Expect.Flagged) == Expect.Flagged;
        public static bool IsProcessed(this Expect expect) => (expect & Expect.Processed) == Expect.Processed;
        public static bool IsToProcess(this Expect expect) => (expect & Expect.NewToProcess) == Expect.NewToProcess;
    }
}
