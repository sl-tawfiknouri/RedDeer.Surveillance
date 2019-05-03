using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Surveillance.Api.Client.Infrastructure
{
    public class Request : IRequest
    {
        private readonly GraphQLClient _client;

        public Request(string url, string bearer, HttpMessageHandler httpMessageHandler)
        {
            _client = new GraphQLClient(url, new GraphQLClientOptions
            {
                HttpMessageHandler = httpMessageHandler
            });
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {bearer}");
        }

        public async Task<GraphQLResponse> PostAsync(GraphQLRequest request, CancellationToken ctx)
        {
            var response = await _client.PostAsync(request, ctx);

            if (response == null)
            {
                throw new Exception("No response from graphql request");
            }
            if (response.Errors?.Any() ?? false)
            {
                throw new Exception($"GraphQL Request Errors ({response.Errors.Length}) {JsonConvert.SerializeObject(response.Errors)}");
            }

            return response;
        }

    }
}
