using System;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeHighProfitFactory : IFixedIncomeHighProfitFactory
    {
        public IFixedIncomeHighProfitsRule BuildRule()
        {
            throw new ArgumentNullException();
        }
    }
}
