using Domain.Equity.Trading;

namespace TestHarness.Engine.OrderGenerator.Strategies
{
    public interface ITradeStrategy
    {
        void ExecuteTradeStrategy(ExchangeFrame tick, ITradeOrderStream tradeOrders);
    }
}
