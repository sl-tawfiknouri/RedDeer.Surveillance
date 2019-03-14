using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Core.Financial.Assets;
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

        public SecurityExposure ExposureToInstrument(FinancialInstrument instrument)
        {
            return SecurityExposure?.FirstOrDefault(i => Equals(i.Instrument, instrument));
        }
    }
}
