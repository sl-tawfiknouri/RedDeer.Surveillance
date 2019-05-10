using GraphQL.Common.Request;
using Surveillance.Api.Client.Infrastructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public abstract class Query<Z, R> : NodeParent where Z : class
    {
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        protected Z AddArgument(string name, object value)
        {
            _arguments[name] = value;
            return this as Z;
        }

        internal abstract Task<R> HandleAsync(IRequest request, CancellationToken ctx);

        protected async Task<T> BuildAndPost<T>(string name, NodeParent node, IRequest request, CancellationToken ctx)
        {
            var query = "{ " + node.Build(name, _arguments) + " }";
            var response = await request.PostAsync(new GraphQLRequest { Query = query }, ctx);
            var result = response.GetDataFieldAs<T>(name);
            return result;
        }
    }
}
