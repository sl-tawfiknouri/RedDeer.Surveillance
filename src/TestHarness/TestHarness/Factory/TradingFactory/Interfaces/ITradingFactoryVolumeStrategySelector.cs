// ReSharper disable UnusedMember.Global

namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryVolumeStrategySelector
    {
        ITradingFactoryFilterStrategySelector TradingFixedVolume(int fixedVolume);

        ITradingFactoryFilterStrategySelector TradingNormalDistributionVolume(int sd);
    }
}