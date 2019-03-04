namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IQueryableMarketDataResponse
    {
        bool HadMissingData();
        Money? PriceOrClose();
    }
}
