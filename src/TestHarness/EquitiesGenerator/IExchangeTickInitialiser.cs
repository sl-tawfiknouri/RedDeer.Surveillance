using Domain.Equity.Trading;

namespace TestHarness.EquitiesGenerator
{
    public interface IExchangeTickInitialiser
    {
        ExchangeTick InitialTick();
    }
}