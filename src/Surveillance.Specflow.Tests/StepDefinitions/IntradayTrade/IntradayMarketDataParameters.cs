using System;

namespace Surveillance.Specflow.Tests.StepDefinitions.IntradayTrade
{
    public class IntradayMarketDataParameters
    {
        public string SecurityName { get; set; }
        public DateTime Epoch { get; set; }

        public decimal? Bid { get; set; }
        public decimal? Ask { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }

        public long? Volume { get; set; }
    }
}
