using System.Collections.Generic;
using Domain.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<ExchangeFrame> OrderedDailyFrames();
    }
}