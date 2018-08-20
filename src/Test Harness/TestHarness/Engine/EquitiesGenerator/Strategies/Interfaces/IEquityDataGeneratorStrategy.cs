using Domain.Equity.Trading;
using Domain.Equity.Trading.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Strategies.Interfaces
{
    public interface IEquityDataGeneratorStrategy
    {
        SecurityFrame AdvanceFrame(SecurityFrame frame);
    }
}