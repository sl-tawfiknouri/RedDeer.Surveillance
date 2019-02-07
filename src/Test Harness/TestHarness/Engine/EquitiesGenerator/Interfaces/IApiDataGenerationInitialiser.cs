using System.Collections.Generic;
using DomainV2.Equity.TimeBars;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames();
    }
}