namespace TestHarness.Factory.TradingLayeringFactory.Interfaces
{
    using System.Collections.Generic;

    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.Plans;

    public interface ITradingLayeringFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<DataGenerationPlan> plan);
    }
}