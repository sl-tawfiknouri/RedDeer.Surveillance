namespace Domain.Market
{
    public interface IMarket
    {
        Market.MarketId Id { get; }
        string Name { get; }
    }
}