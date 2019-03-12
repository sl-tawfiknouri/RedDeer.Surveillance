using System.Collections.Generic;

namespace Domain.Core.Trading.Interfaces
{
    public interface ITradingExposure
    {
        IOrderLedger OrderLedger { get; }
        IReadOnlyCollection<SecurityExposure> SecurityExposure { get; }
    }
}
