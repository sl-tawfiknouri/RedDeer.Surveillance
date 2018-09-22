using Surveillance.Rules.High_Profits.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighProfitRuleFactory
    {
        IHighProfitRule Build();
    }
}