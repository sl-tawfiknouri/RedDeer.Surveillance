using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Surveillance.Api.Client.Filters
{
    public class RuleBreachFilter<T> : Filter<RuleBreachFilter<T>, T> where T : Parent
    {
        public RuleBreachFilter(T node) : base(node) { }

        public RuleBreachFilter<T> ArgumentId(int id) => AddArgument("id", id);
    }
}
