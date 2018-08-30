using Nest;
using Surveillance.ElasticSearchDtos.Interfaces;
using System;

namespace Surveillance.ElasticSearchDtos.Market
{
    [ElasticsearchType(Name = "market-reddeer-document")]
    public class ReddeerMarketDocument : ITraceableDocument
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
        /// Identifier for the market
        /// </summary>
        [Keyword]
        public string MarketId { get; set; }

        /// <summary>
        /// Name of the market
        /// </summary>
        [Text]
        public string MarketName { get; set; }

        /// <summary>
        /// The time point the market update relates to
        /// </summary>
        [Text]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The securities traded in the market
        /// </summary>
        [Object]
        public ReddeerSecurityDocument[] Securities { get; set; }
    }
}
