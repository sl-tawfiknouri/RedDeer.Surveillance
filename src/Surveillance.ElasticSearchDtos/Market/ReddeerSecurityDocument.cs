using Nest;

namespace Surveillance.ElasticSearchDtos.Market
{
    [ElasticsearchType(Name = "trade-security-reddeer-document")]
    public class ReddeerSecurityDocument
    {
        [Keyword]
        public string SecurityId { get; set; }

        [Text]
        public string SecurityName { get; set; }

        [Text]
        public decimal? SpreadBuy { get; set; }

        [Text]
        public decimal? SpreadSell { get; set; }

        [Text]
        public int? Volume { get; set; }
    }
}
