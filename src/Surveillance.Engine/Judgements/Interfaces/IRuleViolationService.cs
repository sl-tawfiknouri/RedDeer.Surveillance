namespace Surveillance.Engine.Rules.Judgements.Interfaces
{
    using Surveillance.Engine.Rules.Rules.Interfaces;

    public interface IRuleViolationService
    {
        void AddRuleViolation(IRuleBreach ruleBreach);

        void ProcessRuleViolationCache();
    }
}