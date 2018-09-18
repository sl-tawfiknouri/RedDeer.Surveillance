namespace Domain.Market.Interfaces
{
    public interface IMarket
    {
        Market.MarketId Id { get; }
        string Name { get; }
    }
}