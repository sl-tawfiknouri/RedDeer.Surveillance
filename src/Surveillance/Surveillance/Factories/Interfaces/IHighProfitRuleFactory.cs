using Surveillance.Rules.High_Profits.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface IHighProfitRuleFactory
    {
        IHighProfitRule Build(IHighProfitsRuleParameters parameters);
    }
}