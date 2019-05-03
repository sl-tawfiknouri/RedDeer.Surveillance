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

    public class RuleRunQuery : Query<Response>
    {
        public RuleRunNode RuleRunNode { get; private set; }

        public RuleRunQuery()
        {
            RuleRunNode = new RuleRunNode(this);
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("ruleRuns", RuleRunNode, request, ctx);
        }
    }
}
