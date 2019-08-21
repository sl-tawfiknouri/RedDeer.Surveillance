namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    using Response = System.Collections.Generic.List<Dtos.AggregationDto>;

    public class OrderAggregationQuery : Query<Response>
    {
        public OrderAggregationQuery()
        {
            this.Filter = new OrderFilter<AggregationNode>(new AggregationNode(this));
        }

        public OrderFilter<AggregationNode> Filter { get; }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<Response>("orderAggregation", this.Filter, request, ctx);
        }
    }
}