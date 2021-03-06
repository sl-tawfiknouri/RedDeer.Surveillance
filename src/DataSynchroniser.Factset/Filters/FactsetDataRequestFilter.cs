﻿namespace DataSynchroniser.Api.Factset.Filters
{
    using DataSynchroniser.Api.Factset.Filters.Interfaces;

    using Domain.Core.Financial.Cfis;

    using SharedKernel.Contracts.Markets;

    public class FactsetDataRequestFilter : IFactsetDataRequestFilter
    {
        public bool ValidAssetType(MarketDataRequest request)
        {
            if (request == null) return false;

            var cfi = new Cfi(request.Cfi);

            return cfi.CfiCategory == CfiCategory.Equities;
        }
    }
}