using System.Collections.Generic;
using System.Threading.Tasks;
using RedDeer.Contracts.SurveillanceService.Api.Markets;

namespace Surveillance.DataLayer.Api.MarketOpenClose.Interfaces
{
    public interface IMarketOpenCloseApiRepository
    {
        Task<IReadOnlyCollection<ExchangeDto>> Get();
    }
}