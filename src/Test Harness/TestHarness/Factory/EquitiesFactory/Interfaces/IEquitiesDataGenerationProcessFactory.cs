using System.Collections.Generic;
using TestHarness.Engine.EquitiesGenerator.Interfaces;
using TestHarness.Engine.Plans;

namespace TestHarness.Factory.EquitiesFactory.Interfaces
{
    public interface IEquitiesDataGenerationProcessFactory
    {
        IEquitiesDataGenerationMarkovProcess Build(IReadOnlyCollection<DataGenerationPlan> plans = null);
    }
}