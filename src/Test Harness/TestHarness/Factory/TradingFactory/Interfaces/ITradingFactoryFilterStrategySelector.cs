using System.Collections.Generic;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryFilterStrategySelector
    {
        ICompleteSelector FilterSedol(IReadOnlyCollection<string> sedols, bool inclusive);
        ICompleteSelector FilterNone();
    }
}
