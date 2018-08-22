using Domain.Equity.Trading.Streams.Interfaces;
using TestHarness.Display;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradeOrderStreamFactory
    {
        ITradeOrderStream Create();
        ITradeOrderStream CreateDisplayable(IConsole console);
    }
}