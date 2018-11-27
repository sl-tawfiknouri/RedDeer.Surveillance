using System.Collections.Generic;

namespace Surveillance.RuleParameters.Filter
{
    public class RuleFilter
    {
        public RuleFilter()
        {
            Ids = new string[0];
        }

        public RuleFilterType Type { get; set; }
        public IReadOnlyCollection<string> Ids { get; set; }

        public static RuleFilter None()
        {
            return new RuleFilter {Type = RuleFilterType.None, Ids = new string[0]};
        }
    }
}
