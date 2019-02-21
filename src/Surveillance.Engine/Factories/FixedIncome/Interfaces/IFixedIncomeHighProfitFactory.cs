using Surveillance.Engine.Rules.Rules.FixedIncome.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    public interface IFixedIncomeHighProfitFactory
    {
        IFixedIncomeHighProfitsRule BuildRule();
    }
}