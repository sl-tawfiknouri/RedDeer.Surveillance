using System.Collections.Generic;
using Domain.Core.Trading.Interfaces;

namespace Domain.Core.Trading
{
    public class TradingExposure : ITradingExposure
    {
        public TradingExposure(IReadOnlyCollection<SecurityExposure> securityExposure)
        {
            SecurityExposure = securityExposure ?? new SecurityExposure[0];
        }

        public IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }
    }
}
