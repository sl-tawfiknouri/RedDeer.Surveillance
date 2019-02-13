using DomainV2.Financial;

namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    public interface IQueryableMarketDataResponse
    {
        bool HadMissingData();
        CurrencyAmount? PriceOrClose();
    }
}
