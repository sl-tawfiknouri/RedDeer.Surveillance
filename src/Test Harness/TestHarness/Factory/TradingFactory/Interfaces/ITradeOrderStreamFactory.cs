using Domain.Streams.Interfaces;
using Domain.Trading;
using TestHarness.Display.Interfaces;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradeOrderStreamFactory
    {
        IOrderStream<Order> Create();
        IOrderStream<Order> CreateDisplayable(IConsole console);
    }
}