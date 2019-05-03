using GraphQL.Common.Request;
using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public abstract class Query<R> : NodeParent
    {
        internal abstract Task<R> HandleAsync(IRequest request, CancellationToken ctx);

        protected async Task<T> BuildAndPost<T>(string name, Node node, IRequest request, CancellationToken ctx)
        {
            var query = "{ " + node.Build(name) + " }";
            var response = await request.PostAsync(new GraphQLRequest { Query = query }, ctx);
            var result = response.GetDataFieldAs<T>(name);
            return result;
        }
    }
}
