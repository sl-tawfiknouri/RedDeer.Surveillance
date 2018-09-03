using Domain.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityFrame AdvanceFrame(SecurityFrame frame);
    }
}