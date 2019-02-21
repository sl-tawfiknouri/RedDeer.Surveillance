using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Equities.Interfaces
{
    public interface IEquityRuleWashTradeFactory
    {
        IWashTradeRule Build(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx, 
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}