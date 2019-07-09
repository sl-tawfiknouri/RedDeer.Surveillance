using Domain.Surveillance.Scheduling;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Equities.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    public interface IEquityRuleHighProfitFactory
    {
        IHighProfitRule Build(
            IHighProfitsRuleEquitiesParameters equitiesParameters,
            ISystemProcessOperationRunRuleContext ruleCtxStream,
            ISystemProcessOperationRunRuleContext ruleCtxMarket,
            IUniverseDataRequestsSubscriber dataRequestSubscriber,
            ScheduledExecution scheduledExecution);
    }
}