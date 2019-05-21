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
    using Response = List<RuleRunDto>;

    public class RuleRunQuery : Query<Response>
    {
        public RuleRunFilter<RuleRunNode> Filter { get; }

        public RuleRunQuery()
        {
            Filter = new RuleRunFilter<RuleRunNode>(new RuleRunNode(this));
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("ruleRuns", Filter, request, ctx);
        }
    }
}
