using Domain.Financial;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IQueryableMarketDataResponse
    {
        bool HadMissingData();
        CurrencyAmount? PriceOrClose();
    }
}
