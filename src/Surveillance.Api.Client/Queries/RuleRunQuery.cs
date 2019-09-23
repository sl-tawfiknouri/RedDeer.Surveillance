namespace RedDeer.Surveillance.Api.Client.Queries
{
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Filters;
    using RedDeer.Surveillance.Api.Client.Infrastructure;
    using RedDeer.Surveillance.Api.Client.Nodes;

    using Response = System.Collections.Generic.List<Dtos.RuleRunDto>;

    public class RuleRunQuery : Query<Response>
    {
        public RuleRunQuery()
        {
            this.Filter = new RuleRunFilter<RuleRunNode>(new RuleRunNode(this));
        }

        public RuleRunFilter<RuleRunNode> Filter { get; }

        internal override async Task<Response> HandleAsync(IRequest request, CancellationToken ctx)
        {
            return await this.BuildAndPost<Response>("ruleRuns", this.Filter, request, ctx);
        }
    }
}