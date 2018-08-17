using Domain.Equity.Trading;

namespace TestHarness.Engine.EquitiesGenerator.Strategies
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityFrame AdvanceFrame(SecurityFrame frame);
    }
}