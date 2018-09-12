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
        public int? Volume { get; set; }

        [JsonConverter(typeof(ElasticSearchDateTimeConverter))]
        public DateTime TimeStamp { get; set; }
    }
}
