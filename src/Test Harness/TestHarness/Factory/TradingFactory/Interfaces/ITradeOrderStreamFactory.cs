using Domain.Trades.Orders;
using Domain.Trades.Streams.Interfaces;
using TestHarness.Display;

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradeOrderStreamFactory
    {
        ITradeOrderStream<TradeOrderFrame> Create();
        ITradeOrderStream<TradeOrderFrame> CreateDisplayable(IConsole console);
    }
}