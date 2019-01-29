using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DomainV2.Equity.TimeBars;
using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    public interface IReddeerMarketRepository
    {
        Task<IReadOnlyCollection<SecurityEnrichmentDto>> GetUnEnrichedSecurities();
        Task UpdateUnEnrichedSecurities(IReadOnlyCollection<SecurityEnrichmentDto> dtos);
        void Create(EquityIntraDayTimeBarCollection entity);
        Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> GetEquityIntraday(DateTime start, DateTime end, ISystemProcessOperationContext opCtx);

        Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> GetEquityInterDay(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx);

        Task<ReddeerMarketRepository.MarketSecurityIds> CreateAndOrGetSecurityId(MarketDataPair pair);
    }
}