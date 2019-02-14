using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Trading;
using Surveillance.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade.Interfaces
{
    public interface IOrdersRepository
    {
        Task Create(Order entity);
        Task<IReadOnlyCollection<Order>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
    }
}