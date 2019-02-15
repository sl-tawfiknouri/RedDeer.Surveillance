using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;

namespace Surveillance.DataLayer.Aurora.Trade.Interfaces
{
    public interface IOrderAllocationRepository
    {
        Task<List<string>> Create(IReadOnlyCollection<OrderAllocation> entities);
        Task Create(OrderAllocation entity);
        Task<IReadOnlyCollection<OrderAllocation>> Get(IReadOnlyCollection<string> orders);
    }
}