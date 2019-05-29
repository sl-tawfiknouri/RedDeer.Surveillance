using System;
// ReSharper disable UnusedMember.Global

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryHeartbeatSelector
    {
        ITradingFactoryVolumeStrategySelector Regular(TimeSpan frequency);
        ITradingFactoryVolumeStrategySelector Irregular(TimeSpan frequency, int sd);
    }
}
