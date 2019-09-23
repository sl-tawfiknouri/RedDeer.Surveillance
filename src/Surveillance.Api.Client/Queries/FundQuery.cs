namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Dtos;
    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    public class FundQuery : Query<List<FundDto>>
    {
        public FundQuery()
        {
            this.Filter = new FundFilter<FundNode>(new FundNode(this));
        }

        public FundFilter<FundNode> Filter { get; }

        internal override async Task<List<FundDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<List<FundDto>>("funds", this.Filter, request, ctx);
        }
    }
}