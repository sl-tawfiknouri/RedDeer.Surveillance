using Surveillance.ElasticSearchDtos.Market;

namespace Surveillance.DataLayer.ElasticSearch.Interfaces
{
    public interface IMarketIndexNameBuilder
    {
        string GetMarketIndexName(ReddeerMarketDocument doc);
    }
}