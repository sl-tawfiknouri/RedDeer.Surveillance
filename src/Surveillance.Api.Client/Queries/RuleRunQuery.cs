using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
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

        internal override async Task<Response> HandleAsync(IRequest request)
        {
            var response = await request.QueryAsync(builder =>
                builder
                    .Field("ruleRuns", ruleRuns =>
                    {
                        RuleRunNode._actions.ForEach(x => x(ruleRuns));
                    }),
                    _arguments.ToArray());

            return response.ruleRuns.ToObject<Response>();
        }
    }
}
