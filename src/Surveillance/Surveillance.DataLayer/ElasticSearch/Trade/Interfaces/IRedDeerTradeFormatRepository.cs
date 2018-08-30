using System.Threading.Tasks;
using Surveillance.ElasticSearchDtos.Trades;

namespace Surveillance.DataLayer.Trade.Interfaces
{
    public interface IRedDeerTradeFormatRepository
    {
        Task Save(ReddeerTradeDocument document);
    }
}