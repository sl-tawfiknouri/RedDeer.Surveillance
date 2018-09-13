using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.ElasticSearch.Market.Interfaces
{
    public interface IRedDeerMarketExchangeFormatRepository
    {
        Task Save(ReddeerMarketDocument document);

        Task<IReadOnlyCollection<ReddeerMarketDocument>> Get(DateTime start, DateTime end);
    }
}