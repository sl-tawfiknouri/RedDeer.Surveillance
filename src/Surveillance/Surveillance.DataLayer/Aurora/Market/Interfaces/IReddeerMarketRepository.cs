using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Equity.Frames;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    public interface IReddeerMarketRepository
    {
        Task Create(ExchangeFrame entity);
        Task<IReadOnlyCollection<ExchangeFrame>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
    }
}