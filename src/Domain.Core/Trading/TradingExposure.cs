using System;
using System.Collections.Generic;
using Domain.Core.Trading.Interfaces;

namespace Domain.Core.Trading
{
    public class TradingExposure : ITradingExposure
    {
        public TradingExposure(IReadOnlyCollection<SecurityExposure> securityExposure, IOrderLedger orderLedger)
        {
            SecurityExposure = securityExposure ?? new SecurityExposure[0];
            OrderLedger = orderLedger ?? throw new ArgumentNullException(nameof(orderLedger));
        }

        public IOrderLedger OrderLedger { get; }
        public IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }
    }
}
