using GraphQL.Common.Request;
using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    using Response = List<RuleBreachDto>;

    public class RuleBreachQuery : Query<Response>
    {
        public RuleBreachFilter<RuleBreachNode> Filter { get; }

        public RuleBreachQuery()
        {
            Filter = new RuleBreachFilter<RuleBreachNode>(new RuleBreachNode(this));
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("ruleBreaches", Filter, request, ctx);
        }
    }
}
