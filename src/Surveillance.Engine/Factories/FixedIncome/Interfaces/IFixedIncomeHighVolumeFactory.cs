using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    public interface IFixedIncomeHighVolumeFactory
    {
        IFixedIncomeHighVolumeRule BuildRule();
    }
}