using Surveillance.Engine.Rules.Rules.FixedIncome.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Factories.FixedIncome.Interfaces
{
    public interface IFixedIncomeWashTradeFactory
    {
        IFixedIncomeWashTradeRule BuildRule();
    }
}