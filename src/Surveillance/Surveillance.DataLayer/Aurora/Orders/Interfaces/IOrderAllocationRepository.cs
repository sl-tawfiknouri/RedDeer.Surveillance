using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trading;

namespace Surveillance.DataLayer.Aurora.Orders.Interfaces
{
    public interface IOrderAllocationRepository
    {
        Task<List<string>> Create(IReadOnlyCollection<OrderAllocation> entities);
        Task Create(OrderAllocation entity);
        Task<IReadOnlyCollection<OrderAllocation>> Get(IReadOnlyCollection<string> orderIds);
        Task<IReadOnlyCollection<OrderAllocation>> GetStaleOrderAllocations(DateTime stalenessIndicator);
    }
}