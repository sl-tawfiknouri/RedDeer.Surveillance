namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Domain.Core.Markets.Collections;

    using RedDeer.Contracts.SurveillanceService.Api.SecurityEnrichment;

    using Surveillance.Auditing.Context.Interfaces;

    public interface IReddeerMarketRepository
    {
        void Create(EquityIntraDayTimeBarCollection entity);

        Task<ReddeerMarketRepository.MarketSecurityIds> CreateAndOrGetSecurityId(MarketDataPair pair);

        Task<IReadOnlyCollection<EquityInterDayTimeBarCollection>> GetEquityInterDay(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx);

        Task<IReadOnlyCollection<EquityIntraDayTimeBarCollection>> GetEquityIntraday(
            DateTime start,
            DateTime end,
            ISystemProcessOperationContext opCtx);

        Task<IReadOnlyCollection<SecurityEnrichmentDto>> GetUnEnrichedSecurities();

        Task UpdateUnEnrichedSecurities(IReadOnlyCollection<SecurityEnrichmentDto> dtos);
    }
}