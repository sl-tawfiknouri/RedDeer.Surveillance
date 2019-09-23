namespace Domain.Surveillance.Rules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Domain.Surveillance.Rules.Interfaces;
    using Domain.Surveillance.Scheduling;

    public class ActiveRulesService : IActiveRulesService
    {
        private readonly IReadOnlyCollection<Rules> _enabledRules = new List<Rules>
                                                                        {
                                                                            Rules.Spoofing,
                                                                            Rules.CancelledOrders,
                                                                            Rules.HighProfits,
                                                                            Rules.MarkingTheClose,
                                                                            Rules.Layering,
                                                                            Rules.HighVolume,
                                                                            Rules.WashTrade,
                                                                            Rules.PaintingTheTape,
                                                                            Rules.FixedIncomeWashTrades,
                                                                            Rules.FixedIncomeHighProfits,
                                                                            Rules.FixedIncomeHighVolumeIssuance,
                                                                            Rules.Ramping,
                                                                            Rules.PlacingOrderWithNoIntentToExecute
                                                                        };

        public IReadOnlyCollection<Rules> DisabledRules()
        {
            var rules = Enum.GetValues(typeof(Rules));
            var castRules = rules.Cast<Rules>().ToList();
            var ruleList = new List<Rules>(castRules);

            return ruleList.Where(i => !this._enabledRules.Contains(i)).ToList();
        }

        public IReadOnlyCollection<Rules> EnabledRules()
        {
            return this._enabledRules;
        }

        public bool RuleIsEnabled(Rules rule)
        {
            return this._enabledRules.Contains(rule);
        }
    }
}