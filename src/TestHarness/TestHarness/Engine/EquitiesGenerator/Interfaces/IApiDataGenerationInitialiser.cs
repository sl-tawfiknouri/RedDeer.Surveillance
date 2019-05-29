using System.Collections.Generic;
using Domain.Core.Markets.Collections;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames();
    }
}