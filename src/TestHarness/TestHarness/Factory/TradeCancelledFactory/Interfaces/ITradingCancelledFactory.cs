namespace TestHarness.Factory.TradeCancelledFactory.Interfaces
{
    using TestHarness.Engine.OrderGenerator.Interfaces;

    public interface ITradingCancelledFactory
    {
        IOrderDataGenerator Create();
    }
}