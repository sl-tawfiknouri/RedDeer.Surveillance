using System;

namespace RedDeer.Surveillance.Api.Client.Dtos
{
    public class TickPriceHistoryTimeBarDto
    {
        public string Ric { get; set; }

        public DateTime EpochUtc { get; set; }

        public string CurrencyCode { get; set; }

        public decimal? Close { get; set; }

        public decimal? CloseAsk { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? HighAsk { get; set; }
    }
}