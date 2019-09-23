namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    using Domain.Surveillance.Streams.Interfaces;

    public interface IEquityDataGenerator
    {
        void InitiateWalk(IStockExchangeStream stream);

        void TerminateWalk();
    }
}