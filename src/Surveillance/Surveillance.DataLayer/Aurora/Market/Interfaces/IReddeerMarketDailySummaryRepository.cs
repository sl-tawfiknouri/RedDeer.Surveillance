using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;

namespace Surveillance.DataLayer.Aurora.Market.Interfaces
{
    public interface IReddeerMarketDailySummaryRepository
    {
        Task Save(IReadOnlyCollection<FactsetSecurityDailyResponseItem> responseItems);
    }
}