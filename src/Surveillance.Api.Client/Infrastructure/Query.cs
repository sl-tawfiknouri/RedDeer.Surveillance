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
    public abstract class Query<R> : NodeParent
    {
        private readonly Dictionary<string, object> _arguments = new Dictionary<string, object>();

        protected T AddArgument<T>(string name, object value, T self) where T : Query<R>
        {
            if (self != this)
            {
                throw new Exception("Self must be set to this");
            }
            _arguments[name] = value;
            return self;
        }

        public T Self<T>() where T : Query<R>
        {
            return this as T;
        }

        internal abstract Task<R> HandleAsync(IRequest request, CancellationToken ctx);

        protected async Task<T> BuildAndPost<T>(string name, Node node, IRequest request, CancellationToken ctx)
        {
            var query = "{ " + node.Build(name, _arguments) + " }";
            var response = await request.PostAsync(new GraphQLRequest { Query = query }, ctx);
            var result = response.GetDataFieldAs<T>(name);
            return result;
        }
    }
}
