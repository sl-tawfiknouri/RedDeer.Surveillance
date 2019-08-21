namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    using Response = System.Collections.Generic.List<Dtos.ClientAccountDto>;

    public class ClientAccountQuery : Query<Response>
    {
        public ClientAccountQuery()
        {
            this.Filter = new ClientAccountFilter<ClientAccountNode>(new ClientAccountNode(this));
        }

        public ClientAccountFilter<ClientAccountNode> Filter { get; }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<Response>("clientAccounts", this.Filter, request, ctx);
        }
    }
}