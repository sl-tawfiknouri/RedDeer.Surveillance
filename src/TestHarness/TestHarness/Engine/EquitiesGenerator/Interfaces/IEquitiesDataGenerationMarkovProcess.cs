namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    using Domain.Surveillance.Streams.Interfaces;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;
    using RedDeer.Contracts.SurveillanceService.Api.SecurityPrices;

    public interface IEquitiesDataGenerationMarkovProcess
    {
        void InitiateWalk(IStockExchangeStream stream, ExchangeDto market, SecurityPriceResponseDto prices);

        void TerminateWalk();
    }
}