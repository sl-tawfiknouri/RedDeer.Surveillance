namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    using System.Collections.Generic;

    using Domain.Core.Markets.Collections;

    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<EquityIntraDayTimeBarCollection> OrderedDailyFrames();
    }
}