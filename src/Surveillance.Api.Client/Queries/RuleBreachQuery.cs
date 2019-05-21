using GraphQL.Common.Request;
using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Filters;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
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
