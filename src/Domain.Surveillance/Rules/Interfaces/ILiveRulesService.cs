using System.Collections.Generic;
using Domain.Surveillance.Scheduling;

namespace Domain.Surveillance.Rules
{
    public interface ILiveRulesService
    {
        IReadOnlyCollection<Scheduling.Rules> LiveRules();
        bool RuleIsLive(Scheduling.Rules rule);
        IReadOnlyCollection<Scheduling.Rules> UnliveRules();
    }
}