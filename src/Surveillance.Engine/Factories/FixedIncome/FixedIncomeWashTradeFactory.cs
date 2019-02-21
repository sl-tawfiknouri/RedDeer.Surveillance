using System;
using Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces;
using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome
{
    public class FixedIncomeWashTradeFactory : IFixedIncomeWashTradeFactory
    {
        public IFixedIncomeWashTradeRule BuildRule()
        {
            throw new ArgumentNullException();
        }
    }
}
