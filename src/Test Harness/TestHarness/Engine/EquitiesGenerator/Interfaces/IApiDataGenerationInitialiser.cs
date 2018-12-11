using System.Collections.Generic;
using DomainV2.Equity.Frames;

namespace TestHarness.Engine.EquitiesGenerator.Interfaces
{
    public interface IApiDataGenerationInitialiser
    {
        IReadOnlyCollection<ExchangeFrame> OrderedDailyFrames();
    }
}