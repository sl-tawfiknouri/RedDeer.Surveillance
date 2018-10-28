using TestHarness.Engine.EquitiesGenerator.Interfaces;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IEquitiesDataGenerationProcessFactory
    {
        IEquitiesDataGenerationMarkovProcess Build();
    }
}