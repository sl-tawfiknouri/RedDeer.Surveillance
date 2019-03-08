using System.Collections.Generic;

namespace Domain.Surveillance.Rules.Interfaces
{
    public interface IActiveRulesService
    {
        IReadOnlyCollection<Scheduling.Rules> EnabledRules();
        bool RuleIsEnabled(Scheduling.Rules rule);
        IReadOnlyCollection<Scheduling.Rules> DisabledRules();
    }
}