using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using Domain.Financial.Cfis;
using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Filters
{
    public class MarketDataRequestFilter : IMarketDataRequestFilter
    {
        public bool Filter(MarketDataRequest request)
        {
            if (request == null)
            {
                return true;
            }

            var cfi = new Cfi(request.Cfi);

            return cfi.CfiCategory != CfiCategory.Equities;
        }
    }
}
