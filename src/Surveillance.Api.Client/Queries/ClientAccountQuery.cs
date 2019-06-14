using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    using Response = List<ClientAccountDto>;

    public class ClientAccountQuery : Query<Response>
    {
        public ClientAccountFilter<ClientAccountNode> Filter { get; }

        public ClientAccountQuery()
        {
            Filter = new ClientAccountFilter<ClientAccountNode>(new ClientAccountNode(this));
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("clientAccounts", Filter, request, ctx);
        }
    }
}
