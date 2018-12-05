using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trades.Orders;
using DomainV2.Trading;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade.Interfaces
{
    public interface IReddeerTradeRepository
    {
        Task Create(Order entity);
        Task<IReadOnlyCollection<Order>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
    }
}