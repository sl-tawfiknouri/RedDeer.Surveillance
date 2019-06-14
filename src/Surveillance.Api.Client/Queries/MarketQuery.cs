using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    public class MarketQuery : Query<List<MarketDto>>
    {
        public FundFilter<MarketNode> Filter { get; }

        public MarketQuery()
        {
            Filter = new FundFilter<MarketNode>(new MarketNode(this));
        }

        internal override async Task<List<MarketDto>> HandleAsync(IRequest request, CancellationToken ct)
        {
            return await BuildAndPost<List<MarketDto>>("markets", Filter, request, ct);
        }
    }
}
