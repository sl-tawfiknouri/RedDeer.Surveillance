using System.Collections.Generic;

namespace Domain.Surveillance.Rules.Interfaces
{
    public interface ILiveRulesService
    {
        IReadOnlyCollection<Scheduling.Rules> LiveRules();
        bool RuleIsLive(Scheduling.Rules rule);
        IReadOnlyCollection<Scheduling.Rules> UnLiveRules();
    }
}