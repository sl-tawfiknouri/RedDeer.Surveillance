namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Dtos;
    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    public class TraderQuery : Query<List<TraderDto>>
    {
        public TraderQuery()
        {
            this.Filter = new TraderFilter<TraderNode>(new TraderNode(this));
        }

        public TraderFilter<TraderNode> Filter { get; }

        internal override async Task<List<TraderDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<List<TraderDto>>("traders", this.Filter, request, ctx);
        }
    }
}