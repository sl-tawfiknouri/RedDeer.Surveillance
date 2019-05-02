using SAHB.GraphQLClient.Executor;
using Surveillance.Api.Client.Infrastructure;
using Surveillance.Api.Client.Queries;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Surveillance.Api.Client
{
    public class ApiClient
    {
        private Request _request;

        public ApiClient(string url, string bearer, HttpMessageHandler httpMessageHandler = null)
        {
            _request = new Request(url, bearer, httpMessageHandler);
        }

        public async Task<R> QueryAsync<R>(Query<R> query)
        {
            return await query.HandleAsync(_request);
        }

    }
}
