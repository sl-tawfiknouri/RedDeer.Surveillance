using System;

namespace Surveillance.Specflow.Tests.StepDefinitions.InterdayTrade
{
    public class InterdayMarketDataParameters
    {
        public string SecurityName { get; set; }
        public DateTime Epoch { get; set; }

        public decimal? MarketCap { get; set; }
        public long? ListedSecurities { get; set; }
        public long? DailyVolume { get; set; }

        public decimal? Open { get; set; }
        public decimal? Close { get; set; }
        public decimal? High { get; set; }
        public decimal? Low { get; set; }

        public string Currency { get; set; }
    }
}
