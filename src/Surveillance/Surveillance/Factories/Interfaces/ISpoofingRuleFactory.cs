using Surveillance.Rules.Spoofing.Interfaces;

namespace Surveillance.Factories.Interfaces
{
    public interface ISpoofingRuleFactory
    {
        ISpoofingRule Build();
    }
}