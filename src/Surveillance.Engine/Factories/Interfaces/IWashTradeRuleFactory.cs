using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.RuleParameters.Interfaces;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Rules.WashTrade.Interfaces;
using Surveillance.Systems.Auditing.Context.Interfaces;

namespace Surveillance.Engine.Rules.Factories.Interfaces
{
    public interface IWashTradeRuleFactory
    {
        IWashTradeRule Build(
            IWashTradeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext ruleCtx, 
            IUniverseAlertStream alertStream,
            RuleRunMode runMode);
    }
}