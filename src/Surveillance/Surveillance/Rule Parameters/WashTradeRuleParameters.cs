using System;
using System.Collections.Generic;
using System.Text;
using Surveillance.Rule_Parameters.Filter;
using Surveillance.Rule_Parameters.Interfaces;

namespace Surveillance.Rule_Parameters
{
    public class WashTradeRuleParameters : IWashTradeRuleParameters
    {
        public WashTradeRuleParameters(
            TimeSpan windowSize)
        {
            WindowSize = windowSize;

            Accounts = RuleFilter.None();
            Traders = RuleFilter.None();
            Markets = RuleFilter.None();
        }

        public TimeSpan WindowSize { get; set; }


        public RuleFilter Accounts { get; set; }
        public RuleFilter Traders { get; set; }
        public RuleFilter Markets { get; set; }

        public bool HasFilters()
        {
            return
                Accounts?.Type != RuleFilterType.None
                || Traders?.Type != RuleFilterType.None
                || Markets?.Type != RuleFilterType.None;
        }
    }
}
