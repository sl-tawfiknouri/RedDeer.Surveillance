using RedDeer.Surveillance.Api.Client.Dtos;
using RedDeer.Surveillance.Api.Client.Filters;
using RedDeer.Surveillance.Api.Client.Infrastructure;
using RedDeer.Surveillance.Api.Client.Nodes;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Queries
{
    using Response = List<OrderDto>;

    public class OrderQuery : Query<Response>
    {
        public OrderFilter<OrderNode> Filter { get; }

        public OrderQuery()
        {
            Filter = new OrderFilter<OrderNode>(new OrderNode(this));
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("orders", Filter, request, ctx);
        }
    }
}
