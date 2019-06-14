using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Rules.Interfaces;

namespace Domain.Surveillance.Rules
{
    public class ActiveRulesService : IActiveRulesService
    {
        private readonly IReadOnlyCollection<Scheduling.Rules> _enabledRules =
            new List<Scheduling.Rules>
            {
                Scheduling.Rules.Spoofing,
                Scheduling.Rules.CancelledOrders,
                Scheduling.Rules.HighProfits,
                Scheduling.Rules.MarkingTheClose,
                Scheduling.Rules.Layering,
                Scheduling.Rules.HighVolume,
                Scheduling.Rules.WashTrade,
                Scheduling.Rules.PaintingTheTape,
                Scheduling.Rules.FixedIncomeWashTrades,
                Scheduling.Rules.FixedIncomeHighProfits,
                Scheduling.Rules.FixedIncomeHighVolumeIssuance,
                Scheduling.Rules.Ramping,
                Scheduling.Rules.PlacingOrderWithNoIntentToExecute
            };

        public bool RuleIsEnabled(Scheduling.Rules rule)
        {
            return _enabledRules.Contains(rule);
        }

        public IReadOnlyCollection<Scheduling.Rules> EnabledRules()
        {
            return _enabledRules;
        }

        public IReadOnlyCollection<Scheduling.Rules> DisabledRules()
        {
            var rules = Enum.GetValues(typeof(Scheduling.Rules));
            var castRules = rules.Cast<Scheduling.Rules>().ToList();
            var ruleList = new List<Scheduling.Rules>(castRules);

            return ruleList.Where(i => !_enabledRules.Contains(i)).ToList();
        }
    }
}
