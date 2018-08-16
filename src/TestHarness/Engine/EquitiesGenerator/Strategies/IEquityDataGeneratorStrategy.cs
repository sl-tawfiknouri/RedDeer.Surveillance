using Domain.Equity.Trading;

namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityTick TickSecurity(SecurityTick tick);
    }
}