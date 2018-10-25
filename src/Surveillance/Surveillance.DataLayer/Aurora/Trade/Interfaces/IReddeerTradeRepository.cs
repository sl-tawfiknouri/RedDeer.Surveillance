using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Trades.Orders;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Trade.Interfaces
{
    public interface IReddeerTradeRepository
    {
        Task Create(TradeOrderFrame entity);
        Task<IReadOnlyCollection<TradeOrderFrame>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
    }
}