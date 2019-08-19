namespace TestHarness.Factory.TradeWashTradeFactory.Interfaces
{
    using TestHarness.Engine.OrderGenerator.Interfaces;
    using TestHarness.Engine.Plans;

    public interface ITradeWashTradeFactory
    {
        IOrderDataGenerator Build(DataGenerationPlan plan);
    }
}