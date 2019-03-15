using System.Collections.Generic;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryFilterStrategySelector
    {
        ICompleteSelector SetFilterSedol(IReadOnlyCollection<string> sedols, bool inclusive);
        ICompleteSelector SetFilterNone();
    }
}
