using System;

namespace TestHarness.Engine.Plans
{
    public class IntervalEquityPriceInstruction
    {
        public IntervalEquityPriceInstruction(
            string sedol,
            TimeSpan intervalCommencement,
            TimeSpan intervalTermination,
            TimeSpan updateFrequency,
            DateTime commenceInUtc,
            DateTime terminationInUtc,
            PriceManipulation priceManipulation,
            decimal? priceTickData)
        {
            Sedol = sedol ?? string.Empty;
            IntervalCommencement = intervalCommencement;
            IntervalTermination = intervalTermination;
            UpdateFrequency = updateFrequency;
            CommencementInUtc = commenceInUtc;
            TerminationInUtc = terminationInUtc;
            PriceManipulation = priceManipulation;
            PriceTickDelta = priceTickData;
        }

        /// <summary>
        /// Identifier of security to manipulate price for
        /// </summary>
        public string Sedol { get; }

        /// <summary>
        /// From
        /// </summary>
        public TimeSpan IntervalCommencement { get; }

        /// <summary>
        /// To
        /// </summary>
        public TimeSpan IntervalTermination { get; }

        /// <summary>
        /// Frequency of equity price updates for the interval
        /// </summary>
        public TimeSpan UpdateFrequency { get; }

        public PriceManipulation PriceManipulation { get; }

        /// <summary>
        /// A fraction to represent the per tick update change i.e. 0.01m is 1% in the direction of price manipulation
        /// </summary>
        public decimal? PriceTickDelta { get; }

        public DateTime CommencementInUtc { get; }
        public DateTime TerminationInUtc { get; }
    }
}
