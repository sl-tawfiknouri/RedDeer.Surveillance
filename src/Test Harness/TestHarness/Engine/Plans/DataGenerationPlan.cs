using System;
using System.Collections.Generic;

namespace TestHarness.Engine.Plans
{
    public class DataGenerationPlan
    {
        public DataGenerationPlan(
            string sedol,
            IntervalEquityPriceInstruction equityInstructions)
        {
            Sedol = sedol ?? string.Empty;
            EquityInstructions = equityInstructions ?? throw new ArgumentNullException(nameof(equityInstructions));
        }

        public string Sedol { get; }

        public IntervalEquityPriceInstruction EquityInstructions { get; }
    }
}
