namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    using Response = System.Collections.Generic.List<Dtos.RuleBreachDto>;

    public class RuleBreachQuery : Query<Response>
    {
        public RuleBreachQuery()
        {
            this.Filter = new RuleBreachFilter<RuleBreachNode>(new RuleBreachNode(this));
        }

        public RuleBreachFilter<RuleBreachNode> Filter { get; }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<Response>("ruleBreaches", this.Filter, request, ctx);
        }
    }
}