using System.Collections.Generic;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces
{
    public interface IPlacingOrdersWithNoIntentToExecuteRuleBreach : IRuleBreach
    {
        IPlacingOrderWithNoIntentToExecuteRuleEquitiesParameters Parameters { get; set; }
        decimal MeanPrice { get; set; }
        decimal StandardDeviationPrice { get; set; }
        IReadOnlyCollection<PlacingOrderWithNoIntentToExecuteRuleRuleBreach.ProbabilityOfExecution> ProbabilityForOrders { get; set; }
    }
}
