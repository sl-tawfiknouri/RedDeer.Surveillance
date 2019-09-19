namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    using Domain.Core.Trading.Interfaces;

    public interface ITradePositionCancellations : ITradePosition
    {
        decimal CancellationRatioByPositionSize();

        decimal CancellationRatioByTradeCount();

        bool HighCancellationRatioByPositionSize();

        bool HighCancellationRatioByTradeCount();
    }
}