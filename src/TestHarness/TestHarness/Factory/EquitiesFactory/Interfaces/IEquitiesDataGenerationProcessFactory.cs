namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    using System.Collections.Generic;

    using TestHarness.Engine.EquitiesGenerator.Interfaces;
    using TestHarness.Engine.Plans;

    public interface IEquitiesDataGenerationProcessFactory
    {
        IEquitiesDataGenerationMarkovProcess Build(IReadOnlyCollection<DataGenerationPlan> plans = null);
    }
}