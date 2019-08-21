namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    using Response = System.Collections.Generic.List<Dtos.OrderDto>;

    public class OrderQuery : Query<Response>
    {
        public OrderQuery()
        {
            this.Filter = new OrderFilter<OrderNode>(new OrderNode(this));
        }

        public OrderFilter<OrderNode> Filter { get; }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<Response>("orders", this.Filter, request, ctx);
        }
    }
}