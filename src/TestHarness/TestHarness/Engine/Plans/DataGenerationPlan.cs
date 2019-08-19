namespace TestHarness.Engine.Plans
{
    using System;

    public class DataGenerationPlan
    {
        public DataGenerationPlan(string sedol, IntervalEquityPriceInstruction equityInstructions)
        {
            this.Sedol = sedol ?? string.Empty;
            this.EquityInstructions = equityInstructions ?? throw new ArgumentNullException(nameof(equityInstructions));
        }

        public IntervalEquityPriceInstruction EquityInstructions { get; }

        public string Sedol { get; }
    }
}