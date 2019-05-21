using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    public interface IRequest
    {
        Task<GraphQLResponse> PostAsync(GraphQLRequest request, CancellationToken ctx);
    }
}
