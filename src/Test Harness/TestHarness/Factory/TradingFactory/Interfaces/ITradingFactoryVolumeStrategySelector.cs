// ReSharper disable UnusedMember.Global
namespace TestHarness.Factory.TradingFactory.Interfaces
{
    public interface ITradingFactoryVolumeStrategySelector
    {
        ICompleteSelector TradingFixedVolume(int fixedVolume);
        ICompleteSelector TradingNormalDistributionVolume(int sd);
    }
}
