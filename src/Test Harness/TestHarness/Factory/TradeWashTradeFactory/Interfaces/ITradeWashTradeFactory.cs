using TestHarness.Engine.OrderGenerator.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Factory.TradeWashTradeFactory.Interfaces
{
    public interface ITradeWashTradeFactory
    {
        IOrderDataGenerator Build(DataGenerationPlan plan);
    }
}