namespace Surveillance.Specflow.Tests.StepDefinitions.InterdayTrade
{
    using System;

    public class InterdayMarketDataParameters
    {
        public decimal? Close { get; set; }

        public string Currency { get; set; }

        public long? DailyVolume { get; set; }

        public DateTime Epoch { get; set; }

        public decimal? High { get; set; }

        public long? ListedSecurities { get; set; }

        public decimal? Low { get; set; }

        public decimal? MarketCap { get; set; }

        public decimal? Open { get; set; }

        public string SecurityName { get; set; }
    }
}