using RedDeer.Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedDeer.Surveillance.Api.Client.Filters
{
    public class RuleRunFilter<T> : Filter<RuleRunFilter<T>, T> where T : Parent
    {
        public RuleRunFilter(T node) : base(node) { }

        public RuleRunFilter<T> ArgumentCorrelationIds(List<string> correlationIds) => AddArgument("correlationIds", correlationIds);
    }
}
