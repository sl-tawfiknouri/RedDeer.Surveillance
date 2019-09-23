namespace Domain.Surveillance.Rules.Interfaces
{
    using System.Collections.Generic;

    using Domain.Surveillance.Scheduling;

    public interface IActiveRulesService
    {
        IReadOnlyCollection<Rules> DisabledRules();

        IReadOnlyCollection<Rules> EnabledRules();

        bool RuleIsEnabled(Rules rule);
    }
}