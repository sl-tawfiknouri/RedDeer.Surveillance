﻿namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using GraphQL.Common.Request;

    public abstract class Query<R> : Parent
    {
        internal override string Build(string name, Dictionary<string, object> arguments)
        {
            throw new NotImplementedException();
        }

        internal abstract Task<R> HandleAsync(IRequest request, CancellationToken ctx);

        protected async Task<T> BuildAndPost<T>(string name, Parent node, IRequest request, CancellationToken ctx)
        {
            var query = "{ " + node.Build(name, null) + " }";
            var response = await request.PostAsync(new GraphQLRequest { Query = query }, ctx);
            var result = response.GetDataFieldAs<T>(name);
            return result;
        }
    }
}