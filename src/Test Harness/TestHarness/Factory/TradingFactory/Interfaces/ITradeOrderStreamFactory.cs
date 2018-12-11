using DomainV2.Streams.Interfaces;
using DomainV2.Trading;
using TestHarness.Display.Interfaces;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradeOrderStreamFactory
    {
        IOrderStream<Order> Create();
        IOrderStream<Order> CreateDisplayable(IConsole console);
    }
}