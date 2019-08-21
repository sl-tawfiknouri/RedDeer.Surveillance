namespace RedDeer.Surveillance.Api.Client.Infrastructure
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using GraphQL.Client;
    using GraphQL.Common.Request;
    using GraphQL.Common.Response;

    using Newtonsoft.Json;

    public class Request : IRequest
    {
        private readonly GraphQLClient _client;

        public Request(string url, string bearer, HttpMessageHandler httpMessageHandler)
        {
            this._client = new GraphQLClient(
                url,
                new GraphQLClientOptions { HttpMessageHandler = httpMessageHandler ?? new HttpClientHandler() });
            this._client.DefaultRequestHeaders.Add("Authorization", $"bearer {bearer}");
        }

        public async Task<GraphQLResponse> PostAsync(GraphQLRequest request, CancellationToken ctx)
        {
            var response = await this._client.PostAsync(request, ctx);

            if (response == null) throw new Exception("No response from graphql request");

            if (response.Errors == null) return response;

            if (response.Errors.Any())
                throw new Exception(
                    $"GraphQL Request Errors ({response.Errors.Length}) {JsonConvert.SerializeObject(response.Errors)}");

            return response;
        }
    }
}