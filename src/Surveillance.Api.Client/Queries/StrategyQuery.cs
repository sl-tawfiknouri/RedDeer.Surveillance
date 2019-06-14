using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    public class StrategyQuery : Query<List<StrategyDto>>
    {
        public StrategyFilter<StrategyNode> Filter { get; }

        public StrategyQuery()
        {
            Filter = new StrategyFilter<StrategyNode>(new StrategyNode(this));
        }

        internal override async Task<List<StrategyDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<List<StrategyDto>>("strategies", Filter, request, ctx);
        }
    }
}
