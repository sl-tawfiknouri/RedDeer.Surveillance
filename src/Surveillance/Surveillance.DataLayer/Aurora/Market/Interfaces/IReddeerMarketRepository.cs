using System.Threading.Tasks;
using Domain.Equity.Frames;

namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    public interface IReddeerMarketRepository
    {
        Task Create(ExchangeFrame entity);
    }
}