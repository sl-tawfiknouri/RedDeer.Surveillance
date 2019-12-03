using GraphQL.Authorization;
using GraphQL.Types;
using Surveillance.Api.App.Authorization;
using System;

namespace Surveillance.Api.App.Types.TickPriceHistory
{
    public class TickPriceHistoryTimeBar
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

    public class TickPriceHistoryTimeBarGraphType 
        : ObjectGraphType<TickPriceHistoryTimeBar>
    {
        public TickPriceHistoryTimeBarGraphType()
        {
            this.AuthorizeWith(PolicyManifest.UserPolicy);
            this.Field(i => i.Ric, true);
            this.Field(i => i.EpochUtc, true).Type(new DateTimeGraphType());
            this.Field(i => i.CurrencyCode, true);
            this.Field(i => i.Close, true);
            this.Field(i => i.CloseAsk, true);
            this.Field(i => i.High, true);
            this.Field(i => i.Low, true);
            this.Field(i => i.HighAsk, true);
        }
    }
}
