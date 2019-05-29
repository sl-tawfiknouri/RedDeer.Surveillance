using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Factory.TradeHighProfitFactory.Interfaces
{
    public interface ITradeHighProfitFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plans);
    }
}