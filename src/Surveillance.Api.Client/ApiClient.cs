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
        private HttpClientHandler _httpClientHandler;

        public ApiClient()
        {
        }

        public ApiClient(HttpClientHandler httpClientHandler)
        {
            _httpClientHandler = httpClientHandler;
        }

        public async Task<int> RuleBreachesCountAsync()
        {
            var client = new GraphQLClient("https://127.0.0.1:8888/graphql/surveillance", new GraphQLClientOptions
            {
                HttpMessageHandler = _httpClientHandler
            });
            client.DefaultRequestHeaders.Add("Authorization", @"bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyZWRkZWVyIjoic3VydmVpbGxhbmNlIHJlYWRlciJ9.aT0XggctM4GAQB58j0spzOgRDr5Onrrz_JBrbDVOizI");
            var response = await client.PostAsync(new GraphQLRequest()
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
