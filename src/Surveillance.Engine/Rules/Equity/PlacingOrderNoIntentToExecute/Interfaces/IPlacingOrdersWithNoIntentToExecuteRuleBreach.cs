namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    using System.Collections.Generic;

    using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IPlacingOrdersWithNoIntentToExecuteRuleBreach : IRuleBreach
    {
        decimal MeanPrice { get; set; }

        IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters Parameters { get; set; }

        IReadOnlyCollection<PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution> ProbabilityForOrders
        {
            get;
            set;
        }

        decimal StandardDeviationPrice { get; set; }
    }
}