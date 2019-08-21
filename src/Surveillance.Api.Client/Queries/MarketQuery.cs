namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Dtos;
    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    public class MarketQuery : Query<List<MarketDto>>
    {
        public MarketQuery()
        {
            this.Filter = new FundFilter<MarketNode>(new MarketNode(this));
        }

        public FundFilter<MarketNode> Filter { get; }

        internal override async Task<List<MarketDto>> HandleAsync(IRequest request, CancellationToken ct)
        {
            return await this.BuildAndPost<List<MarketDto>>("markets", this.Filter, request, ct);
        }
    }
}