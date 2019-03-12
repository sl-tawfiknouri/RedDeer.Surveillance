using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Assets;
using Domain.Core.Trading.Interfaces;

namespace Domain.Core.Trading
{
    public class PortfolioExposure : IPortfolioExposure
    {
        public PortfolioExposure(IReadOnlyCollection<SecurityExposure> holding)
        {
            SecurityExposure = holding ?? new List<SecurityExposure>();
        }

        public IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }

        public SecurityExposure GetHolding(FinancialInstrument instrument)
        {
            return SecurityExposure?.FirstOrDefault(i => Equals(i.Instrument?.Identifiers, instrument?.Identifiers));
        }
    }
}
