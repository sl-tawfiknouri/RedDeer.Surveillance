using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    using Response = List<RuleRunDto>;

    public class RuleRunQuery : Query<RuleRunQuery, Response>
    {
        public RuleRunNode RuleRunNode { get; }

        public RuleRunQuery()
        {
            RuleRunNode = new RuleRunNode(this);
        }

        public RuleRunQuery ArgumentCorrelationIds(List<string> correlationIds) => AddArgument("correlationIds", correlationIds);

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("ruleRuns", RuleRunNode, request, ctx);
        }
    }
}
