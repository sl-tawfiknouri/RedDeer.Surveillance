namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using TestHarness.Engine.OrderGenerator.Interfaces;

    public interface ICompleteSelector
    {
        IOrderDataGenerator Finish();
    }
}