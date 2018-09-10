using Nest;
using Surveillance.ElasticSearchDtos.Interfaces;
using System;

namespace Surveillance.ElasticSearchDtos.Trades
{
    [ElasticsearchType(Name = "trade-reddeer-document")]
    public class ReddeerTradeDocument : ITraceableDocument
    {
        /// <summary>
        /// A GUID for identifying the breach {Guid.TimeStamp}
        /// </summary>
        [Keyword]
        public string Id { get; set; }

        /// <summary>
        /// {machine name.service name} as a trace for originating service of document
        /// </summary>
        [Text]
        public string Origin { get; set; }

        /// <summary>
        /// Enum value for the order type
        /// </summary>
        [Keyword]
        public int OrderTypeId { get; set; }

        /// <summary>
        /// Friendly order type value
        /// </summary>
        [Text]
        public string OrderTypeDescription { get; set; }

        /// <summary>
        /// The identifier for the market the trade was conducted on i.e. LSE
        /// </summary>
        [Keyword]
        public string MarketId { get; set; }

        /// <summary>
        /// A friendly name for the market i.e. London Stock Exchange
        /// </summary>
        [Text]
        public string MarketName { get; set; }

        /// <summary>
        /// Security identifier i.e. STAN
        /// </summary>
        [Keyword]
        public string SecurityId { get; set; }

        /// <summary>
        /// Security name i.e. Standard Chartered
        /// </summary>
        [Keyword]
        public string SecurityName { get; set; }

        /// <summary>
        /// Price limit if it is a limit order
        /// </summary>
        [Text]
        public decimal? Limit { get; set; }

        /// <summary>
        /// The most recent mutation to the trade data i.e. created/overwritten on...
        /// </summary>
        [Text]
        public DateTime StatusChangedOn { get; set; }

        /// <summary>
        /// The quantity metric for the traded security
        /// </summary>
        [Keyword]
        public int Volume { get; set; }

        /// <summary>
        /// The direction of the order i.e. buy or sell
        /// </summary>
        [Keyword]
        public int OrderDirectionId { get; set; }

        /// <summary>
        /// The direction of the order i.e. buy or sell
        /// </summary>
        [Keyword]
        public string OrderDirectionDescription { get; set; }

        /// <summary>
        /// The status of the order
        /// </summary>
        [Keyword]
        public int OrderStatusId { get; set; }

        /// <summary>
        /// Friendly description of the order status i.e. placed, cancelled, fulfilled
        /// </summary>
        [Text]
        public string OrderStatusDescription { get; set; }
    }
}
