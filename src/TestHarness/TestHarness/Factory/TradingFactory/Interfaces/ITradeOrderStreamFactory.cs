namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using Domain.Core.Trading.Orders;
    using Domain.Surveillance.Streams.Interfaces;

    using TestHarness.Display.Interfaces;

    public interface ITradeOrderStreamFactory
    {
        IOrderStream<Order> Create();

        IOrderStream<Order> CreateDisplayable(IConsole console);
    }
}