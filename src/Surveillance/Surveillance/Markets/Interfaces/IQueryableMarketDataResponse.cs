using DomainV2.Financial;

namespace Surveillance.Markets.Interfaces
{
    public interface IQueryableMarketDataResponse
    {
        bool HadMissingData();
        CurrencyAmount? PriceOrClose();
    }
}
