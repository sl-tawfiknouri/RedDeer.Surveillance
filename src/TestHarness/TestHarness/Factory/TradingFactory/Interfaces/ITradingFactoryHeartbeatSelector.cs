// ReSharper disable UnusedMember.Global

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    using System;

    public interface ITradingFactoryHeartbeatSelector
    {
        ITradingFactoryVolumeStrategySelector Irregular(TimeSpan frequency, int sd);

        ITradingFactoryVolumeStrategySelector Regular(TimeSpan frequency);
    }
}