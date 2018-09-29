using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ISpoofingRuleFactory
    {
        ISpoofingRule Build(ISpoofingRuleParameters spoofingParameters);
    }
}