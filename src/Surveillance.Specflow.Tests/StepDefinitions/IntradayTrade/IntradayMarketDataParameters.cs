namespace Surveillance.Specflow.Tests.StepDefinitions.IntradayTrade
{
    using System;

    public class IntradayMarketDataParameters
    {
        public decimal? Ask { get; set; }

        public decimal? Bid { get; set; }

        public string Currency { get; set; }

        public DateTime Epoch { get; set; }

        public decimal? Price { get; set; }

        public string SecurityName { get; set; }

        public long? Volume { get; set; }
    }
}