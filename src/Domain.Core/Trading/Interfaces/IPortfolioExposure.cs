using System.Collections.Generic;
using Domain.Core.Financial.Assets;

namespace Domain.Core.Trading.Interfaces
{
    public interface IPortfolioExposure
    {
        IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }

        SecurityExposure GetHolding(FinancialInstrument instrument);
    }
}