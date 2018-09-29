using System;
using Surveillance.DataLayer.ElasticSearch.DataAccess.Interfaces;
using Surveillance.DataLayer.ElasticSearch.Interfaces;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.ElasticSearch
{
    public class MarketIndexNameBuilder : IMarketIndexNameBuilder
    {
        private readonly IElasticSearchDataAccess _dataAccess;

        public MarketIndexNameBuilder(IElasticSearchDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
        }

        public string GetMarketIndexName(ReddeerMarketDocument doc)
        {
            if (doc == null)
            {
                return string.Empty;
            }

            return $"{_dataAccess.ReddeerStockExchangeFormatIndexName}-{doc.MarketId}";
        }
    }
}
