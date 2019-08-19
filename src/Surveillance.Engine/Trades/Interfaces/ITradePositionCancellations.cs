namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface ITradePositionCancellations : ITradePosition
    {
        decimal CancellationRatioByPositionSize();

        decimal CancellationRatioByTradeCount();

        bool HighCancellationRatioByPositionSize();

        bool HighCancellationRatioByTradeCount();
    }
}