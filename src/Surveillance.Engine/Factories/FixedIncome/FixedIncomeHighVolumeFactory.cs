using System;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighVolume.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeHighVolumeFactory : IFixedIncomeHighVolumeFactory
    {
        public IFixedIncomeHighVolumeRule BuildRule()
        {
            throw new ArgumentNullException();
        }
    }
}
