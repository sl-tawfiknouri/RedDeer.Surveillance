namespace Surveillance.Engine.Rules.Markets.Interfaces
{
    using Domain.Core.Financial.Money;

    public interface IQueryableMarketDataResponse
    {
        bool HadMissingData();

        Money? PriceOrClose();
    }
}