namespace Surveillance.Engine.Rules.Trades.Interfaces
{
    public interface ITradePositionCancellations : ITradePosition
    {
        bool HighCancellationRatioByTradeCount();
        bool HighCancellationRatioByPositionSize();
        decimal CancellationRatioByTradeCount();
        decimal CancellationRatioByPositionSize();
    }
}
