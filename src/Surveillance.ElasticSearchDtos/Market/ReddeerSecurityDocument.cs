using System;
using Nest;
using Newtonsoft.Json;
using Surveillance.ElasticSearchDtos.JsonConverters;

namespace Surveillance.ElasticSearchDtos.Market
{
    [ElasticsearchType(Name = "trade-security-reddeer-document")]
    public class ReddeerSecurityDocument
    {
        [Keyword]
        public string SecurityClientIdentifier { get; set; }

        [Keyword]
        public string SecuritySedol { get; set; }

        [Keyword]
        public string SecurityIsin { get; set; }

        [Keyword]
        public string SecurityFigi { get; set; }

        [Keyword]
        public string SecurityCusip { get; set; }

        [Keyword]
        public string SecurityExchangeSymbol { get; set; }

        [Text]
        public string SecurityCfi { get; set; }

        [Text]
        public string SecurityName { get; set; }

        [Text]
        public decimal? SpreadBuy { get; set; }

        [Text]
        public string SpreadBuyCurrency { get; set; }

        [Text]
        public decimal? SpreadSell { get; set; }

        [Text]
        public string SpreadSellCurrency { get; set; }

        [Text]
        public decimal? SpreadPrice { get; set; }

        [Text]
        public string SpreadPriceCurrency { get; set; }

        [Text]
        public decimal? OpenPrice { get; set; }

        [Text]
        public decimal? ClosePrice { get; set; }

        [Text]
        public decimal? HighPrice { get; set; }

        [Text]
        public decimal? LowPrice { get; set; }

        [Text]
        public int? Volume { get; set; }

        [Text]
        public int? ListedSecurities { get; set; }

        /// <summary>
        /// Time the data was considered cannon
        /// </summary>
        [JsonConverter(typeof(ElasticSearchDateTimeConverter))]
        public DateTime TimeStamp { get; set; }

        [Text]
        public decimal? MarketCap { get; set; }


        [Text]
        public string SecurityLei { get; set;}

        [Text]
        public string SecurityBloombergTicker { get; set; }

        [Text]
        public string IssuerIdentifier { get; set; }
    }
}