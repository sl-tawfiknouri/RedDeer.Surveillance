using GraphQL.Client;
using GraphQL.Common.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Surveillance.Api.Client
{
    public class ApiClient
    {
        public async Task RequestAsync()
        {
            var client = new GraphQLClient("https://127.0.0.1:8888/graphql/surveillance");
            client.DefaultRequestHeaders.Add("Authorization", @"bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyZWRkZWVyIjoic3VydmVpbGxhbmNlIHJlYWRlciJ9.aT0XggctM4GAQB58j0spzOgRDr5Onrrz_JBrbDVOizI");
            var response = await client.PostAsync(new GraphQLRequest()
            {
                Query = @"{ ruleBreaches { id } }"
            });

            if (response.Errors.Any())
            {
                throw new Exception($"GraphQL Request Errors {response.Errors.Length}");
            }
        }
    }
}
