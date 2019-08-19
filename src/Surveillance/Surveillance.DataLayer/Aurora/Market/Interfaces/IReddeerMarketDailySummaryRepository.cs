namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

    public interface IReddeerMarketDailySummaryRepository
    {
        Task Save(IReadOnlyCollection<FactsetSecurityDailyResponseItem> responseItems);
    }
}