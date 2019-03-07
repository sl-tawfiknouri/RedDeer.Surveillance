using Domain.Surveillance.Streams.Interfaces;
using TestHarness.Display.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IStockExchangeStreamFactory
    {
        IStockExchangeStream Create();
        IStockExchangeStream CreateDisplayable(IConsole console);
    }
}