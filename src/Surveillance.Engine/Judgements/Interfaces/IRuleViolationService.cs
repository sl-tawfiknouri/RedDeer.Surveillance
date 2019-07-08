using Surveillance.Engine.Rules.Rules.Interfaces;

namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    public interface IRuleViolationService
    {
        void AddRuleViolation(IRuleBreach ruleBreach);
        void ProcessRuleViolationCache();
    }
}