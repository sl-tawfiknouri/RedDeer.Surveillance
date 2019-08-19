namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Dtos;
    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    public class StrategyQuery : Query<List<StrategyDto>>
    {
        public StrategyQuery()
        {
            this.Filter = new StrategyFilter<StrategyNode>(new StrategyNode(this));
        }

        public StrategyFilter<StrategyNode> Filter { get; }

        internal override async Task<List<StrategyDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<List<StrategyDto>>("strategies", this.Filter, request, ctx);
        }
    }
}