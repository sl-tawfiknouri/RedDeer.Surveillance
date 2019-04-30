using SAHB.GraphQLClient.Builder;
using SAHB.GraphQLClient.QueryGenerator;
using Surveillance.Api.Client.Dtos;
using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Queries
{
    using Response = List<RuleBreachDto>;

    public class RuleBreachQuery : IQuery<Response>
    {
        private List<Action<IGraphQLQueryFieldBuilder>> _actions = new List<Action<IGraphQLQueryFieldBuilder>>();
        private List<GraphQLQueryArgument> _arguments = new List<GraphQLQueryArgument>();

        public RuleBreachQuery ArgumentId(int id)
        {
            _actions.Add(ruleBreaches =>
                ruleBreaches.Argument("id", "", "ruleBreach_id"));
            _arguments.Add(new GraphQLQueryArgument("ruleBreach_id", id));
            return this;
        }

        public async Task<Response> HandleAsync(IRequest request)
        {
            var response = await request.QueryAsync(builder =>
                builder
                    .Field("ruleBreaches", ruleBreaches =>
                    {
                        ruleBreaches
                            .Field("id")
                            .Field("ruleId");
                        
                        foreach (var action in _actions)
                        {
                            action(ruleBreaches);
                        }
                    }), _arguments.ToArray());

            return response.ruleBreaches.ToObject<Response>();
        }
    }
}
