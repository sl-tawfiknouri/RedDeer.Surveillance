using DataSynchroniser.Api.Factset.Filters.Interfaces;
using Domain.Core.Financial.Cfis;
using SharedKernel.Contracts.Markets;

namespace DataSynchroniser.Api.Factset.Filters
{
    public class FactsetDataRequestFilter : IFactsetDataRequestFilter
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
