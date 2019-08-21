namespace TestHarness.Factory.TradingSpoofingFactory.Interfaces
{
    using TestHarness.Engine.OrderGenerator.Interfaces;

    public interface ITradingSpoofingProcessFactory
    {
        IOrderDataGenerator Create();
    }
}