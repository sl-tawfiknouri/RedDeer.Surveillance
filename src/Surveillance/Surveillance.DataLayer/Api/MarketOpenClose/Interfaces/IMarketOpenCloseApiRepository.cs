﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.DataLayer.Api.Interfaces;

namespace Surveillance.DataLayer.Api.MarketOpenClose.Interfaces
{
    public interface IMarketOpenCloseApiRepository : IHeartbeatApi
    {
        Task<IReadOnlyCollection<ExchangeDto>> Get();
    }
}