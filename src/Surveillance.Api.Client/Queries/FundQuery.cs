using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    public class FundQuery : Query<List<FundDto>>
    {
        public FundFilter<FundNode> Filter { get; }

        public FundQuery()
        {
            Filter = new FundFilter<FundNode>(new FundNode(this));
        }

        internal override async Task<List<FundDto>> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<List<FundDto>>("funds", Filter, request, ctx);
        }
    }
}
