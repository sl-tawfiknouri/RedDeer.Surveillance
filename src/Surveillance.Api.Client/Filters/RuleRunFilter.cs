namespace RedDeer.Surveillance.Api.Client.Filters
{
    using System.Collections.Generic;

    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class RuleRunFilter<T> : Filter<RuleRunFilter<T>, T>
        where T : Parent
    {
        public RuleRunFilter(T node)
            : base(node)
        {
        }

        public RuleRunFilter<T> ArgumentCorrelationIds(List<string> correlationIds)
        {
            return this.AddArgument("correlationIds", correlationIds);
        }
    }
}