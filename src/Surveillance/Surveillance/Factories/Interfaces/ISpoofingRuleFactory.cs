using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ISpoofingRuleFactory
    {
        ISpoofingRule Build(ISpoofingRuleParameters spoofingParameters, ISystemProcessOperationRunRuleContext ctx);
    }
}