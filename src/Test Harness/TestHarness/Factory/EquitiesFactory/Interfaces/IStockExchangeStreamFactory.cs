using Domain.Equity.Trading.Streams.Interfaces;
using TestHarness.Display;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IStockExchangeStreamFactory
    {
        IStockExchangeStream Create();
        IStockExchangeStream CreateDisplayable(IConsole console);
    }
}