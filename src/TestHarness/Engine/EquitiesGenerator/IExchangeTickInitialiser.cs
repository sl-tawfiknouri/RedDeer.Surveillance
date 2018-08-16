using Domain.Equity.Trading;

namespace TestHarness.Engine.EquitiesGenerator
{
    public interface IExchangeTickInitialiser
    {
        ExchangeTick InitialTick();
    }
}