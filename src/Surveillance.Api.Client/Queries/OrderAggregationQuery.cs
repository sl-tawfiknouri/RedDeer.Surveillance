using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Filters;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Nodes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    using Response = List<AggregationDto>;

    public class OrderAggregationQuery : Query<Response>
    {
        public OrderFilter<AggregationNode> Filter { get; }

        public OrderAggregationQuery()
        {
            Filter = new OrderFilter<AggregationNode>(new AggregationNode(this));
        }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await BuildAndPost<Response>("orderAggregation", Filter, request, ctx);
        }
    }
}
