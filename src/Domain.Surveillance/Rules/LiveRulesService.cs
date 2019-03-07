using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Surveillance.Rules
{
    public class LiveRulesService : ILiveRulesService
    {
        private readonly IReadOnlyCollection<Scheduling.Rules> _liveRules =
            new List<Scheduling.Rules>
            {
                Scheduling.Rules.HighVolume,
                Scheduling.Rules.CancelledOrders,
                Scheduling.Rules.HighProfits,
                Scheduling.Rules.MarkingTheClose,
                Scheduling.Rules.WashTrade,
                Scheduling.Rules.FixedIncomeWashTrades,
                Scheduling.Rules.FixedIncomeHighProfits,
                Scheduling.Rules.FixedIncomeHighVolumeIssuance
            };

        public bool RuleIsLive(Scheduling.Rules rule)
        {
            return _liveRules.Contains(rule);
        }

        public IReadOnlyCollection<Scheduling.Rules> LiveRules()
        {
            return _liveRules;
        }

        public IReadOnlyCollection<Scheduling.Rules> UnliveRules()
        {
            var rules = Enum.GetValues(typeof(Scheduling.Rules));
            var castRules = rules.Cast<Scheduling.Rules>().ToList();
            var ruleList = new List<Scheduling.Rules>(castRules);

            return ruleList.Where(i => !_liveRules.Contains(i)).ToList();
        }
    }
}
