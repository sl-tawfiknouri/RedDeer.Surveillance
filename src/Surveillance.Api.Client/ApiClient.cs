using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Surveillance.Api.Client
{
    public class ApiClient
    {
        private readonly GraphQLClient _client;

        public ApiClient(HttpClientHandler httpClientHandler)
        {
            /* {
                 "exp": 1577836800,
                 "iss": "dev:test:clientservice",
                 "aud": "dev:test:clientservice",
                 "reddeer": "surveillance reader"
               } */
            var bearer = @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1Nzc4MzY4MDAsImlzcyI6ImRldjp0ZXN0OmNsaWVudHNlcnZpY2UiLCJhdWQiOiJkZXY6dGVzdDpjbGllbnRzZXJ2aWNlIiwicmVkZGVlciI6InN1cnZlaWxsYW5jZSByZWFkZXIifQ.HbvU5W3O5btPQ7Ou5aiyscPuBrRJ6iCm3Jig-QqikBE";

            _client = new GraphQLClient("https://localhost:8888/graphql/surveillance", new GraphQLClientOptions
            {
                HttpMessageHandler = httpClientHandler
            });
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {bearer}");
        }

        public async Task<int> RuleBreachesCountAsync()
        {
            var response = await _client.PostAsync(new GraphQLRequest()
            {
                Query = @"{ ruleBreaches { id } }"
            });

            if (response == null)
            {
                throw new Exception("No response from graphql request");
            }
            if (response.Errors?.Any() ?? false)
            {
                throw new Exception($"GraphQL Request Errors {response.Errors.Length}");
            }

            return response.Data.ruleBreaches.Count;
        }
    }
}
