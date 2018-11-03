using System.Collections.Generic;
using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Factory.TradingLayeringFactory.Interfaces
{
    public interface ITradingLayeringFactory
    {
        IOrderDataGenerator Build(IReadOnlyCollection<string> sedols, IReadOnlyCollection<DataGenerationPlan> plan);
    }
}