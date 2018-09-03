using System.Threading.Tasks;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.ElasticSearch.Trade.Interfaces
{
    public interface IRedDeerTradeFormatRepository
    {
        Task Save(ReddeerTradeDocument document);
    }
}