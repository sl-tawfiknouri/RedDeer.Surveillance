using Domain.Equity.Trading.Frames;
using Domain.Equity.Trading.Streams.Interfaces;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public interface ITradeStrategy
    {
        void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream tradeOrders);
    }
}
