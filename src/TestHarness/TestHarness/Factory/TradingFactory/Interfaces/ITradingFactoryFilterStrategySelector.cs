namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using System.Collections.Generic;

    public interface ITradingFactoryFilterStrategySelector
    {
        ICompleteSelector SetFilterNone();

        ICompleteSelector SetFilterSedol(IReadOnlyCollection<string> sedols, bool inclusive);
    }
}