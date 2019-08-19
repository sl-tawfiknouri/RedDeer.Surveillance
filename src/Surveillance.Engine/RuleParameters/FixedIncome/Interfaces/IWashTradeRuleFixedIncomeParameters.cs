namespace Surveillance.Engine.Rules.RuleParameters.FixedIncome.Interfaces
{
    using Surveillance.Engine.Rules.RuleParameters.Interfaces;

    public interface IWashTradeRuleFixedIncomeParameters : IFilterableRule,
                                                           IOrganisationalFactorable,
                                                           IWashTradeRuleParameters
    {
        TimeWindows Windows { get; }
    }
}