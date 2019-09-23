namespace TestHarness.Engine.Plans
{
    using System;

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
            this.Sedol = sedol ?? string.Empty;
            this.IntervalCommencement = intervalCommencement;
            this.IntervalTermination = intervalTermination;
            this.UpdateFrequency = updateFrequency;
            this.CommencementInUtc = commenceInUtc;
            this.TerminationInUtc = terminationInUtc;
            this.PriceManipulation = priceManipulation;
            this.PriceTickDelta = priceTickData;
        }

        public DateTime CommencementInUtc { get; }

        /// <summary>
        ///     From
        /// </summary>
        public TimeSpan IntervalCommencement { get; }

        /// <summary>
        ///     To
        /// </summary>
        public TimeSpan IntervalTermination { get; }

        public PriceManipulation PriceManipulation { get; }

        /// <summary>
        ///     A fraction to represent the per tick update change i.e. 0.01m is 1% in the direction of price manipulation
        /// </summary>
        public decimal? PriceTickDelta { get; }

        /// <summary>
        ///     Identifier of security to manipulate price for
        /// </summary>
        public string Sedol { get; }

        public DateTime TerminationInUtc { get; }

        /// <summary>
        ///     Frequency of equity price updates for the interval
        /// </summary>
        public TimeSpan UpdateFrequency { get; }
    }
}