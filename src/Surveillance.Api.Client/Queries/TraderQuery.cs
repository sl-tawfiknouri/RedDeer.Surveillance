using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    public class TraderQuery : Query<List<TraderDto>>
    {
        public TraderFilter<TraderNode> Filter { get; }

        public TraderQuery()
        {
            Filter = new TraderFilter<TraderNode>(new TraderNode(this));
        }

        internal override async Task<List<TraderDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<List<TraderDto>>("traders", Filter, request, ctx);
        }
    }
}
