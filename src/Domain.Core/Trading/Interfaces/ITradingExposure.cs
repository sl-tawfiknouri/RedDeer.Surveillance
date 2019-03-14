using System.Collections.Generic;
using Domain.Core.Financial.Assets;

namespace Domain.Core.Trading.Interfaces
{
    public interface ITradingExposure
    {
        IOrderLedger OrderLedger { get; }
        IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }
        SecurityExposure ExposureToInstrument(FinancialInstrument instrument);
    }
}
