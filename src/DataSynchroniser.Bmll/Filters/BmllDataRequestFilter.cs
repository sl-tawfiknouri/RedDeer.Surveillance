using DataSynchroniser.Api.Bmll.Filters.Interfaces;
using Domain.Core.Financial.Cfis;
using Domain.Markets;

namespace DataSynchroniser.Api.Bmll.Filters
{
    public class BmllDataRequestFilter : IBmllDataRequestFilter
    {
        public bool ValidAssetType(MarketDataRequest request)
        {
            if (request == null)
            {
                return false;
            }

            var cfi = new Cfi(request.Cfi);

            return cfi.CfiCategory == CfiCategory.Equities;
        }
    }
}
