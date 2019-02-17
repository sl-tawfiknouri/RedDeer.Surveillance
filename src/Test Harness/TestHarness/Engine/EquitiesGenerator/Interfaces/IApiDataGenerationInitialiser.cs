using System.Collections.Generic;
using Domain.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames();
    }
}