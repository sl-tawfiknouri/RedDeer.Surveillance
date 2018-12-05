using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Equity.Frames;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    public interface IReddeerMarketRepository
    {
        Task<IReadOnlyCollection<SecurityEnrichmentDto>> GetUnEnrichedSecurities();
        Task UpdateUnEnrichedSecurities(IReadOnlyCollection<SecurityEnrichmentDto> dtos);
        Task Create(ExchangeFrame entity);
        Task<IReadOnlyCollection<ExchangeFrame>> Get(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);
        Task<string> CreateAndOrGetSecurityId(MarketDataPair pair);
    }
}