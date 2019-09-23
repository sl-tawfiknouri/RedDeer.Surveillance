namespace TestHarness.Factory.TradeHighProfitFactory.Interfaces
{
    using System.Collections.Generic;

    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.Plans;

    public interface ITradeHighProfitFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plans);
    }
}