using System.Threading.Tasks;
using DomainV2.Trading;

namespace Surveillance.DataLayer.Aurora.Trade
{
    public interface IOrderAllocationRepository
    {
        Task Create(OrderAllocation entity);
        Task<object> Get(object entity);
    }
}