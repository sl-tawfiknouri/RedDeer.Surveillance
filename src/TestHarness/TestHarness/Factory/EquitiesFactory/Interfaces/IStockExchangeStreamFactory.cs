namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using Domain.Surveillance.Streams.Interfaces;

    using TestHarness.Display.Interfaces;

    public interface IStockExchangeStreamFactory
    {
        IStockExchangeStream Create();

        IStockExchangeStream CreateDisplayable(IConsole console);
    }
}