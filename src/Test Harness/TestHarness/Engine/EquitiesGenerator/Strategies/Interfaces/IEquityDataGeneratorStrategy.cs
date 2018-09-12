using Domain.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityTick AdvanceFrame(SecurityTick tick);
    }
}