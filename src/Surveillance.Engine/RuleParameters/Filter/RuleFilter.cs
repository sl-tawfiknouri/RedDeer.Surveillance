namespace Surveillance.Engine.Rules.RuleParameters.Filter
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RuleFilter
    {
        public RuleFilter()
        {
            this.Ids = new string[0];
        }

        public IReadOnlyCollection<string> Ids { get; set; }

        public RuleFilterType Type { get; set; }

        public static RuleFilter None()
        {
            return new RuleFilter { Type = RuleFilterType.None, Ids = new string[0] };
        }
    }
}