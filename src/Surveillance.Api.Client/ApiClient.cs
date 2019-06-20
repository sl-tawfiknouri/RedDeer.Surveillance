using RedDeer.Surveillance.Api.Client.Infrastructure;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.Api.Client
{
    public class ApiClient
    {
        private readonly Request _request;

        public ApiClient(string url, string bearer, HttpMessageHandler httpMessageHandler = null)
        {
            _request = new Request(url, bearer, httpMessageHandler);
        }

        public async Task<R> QueryAsync<R>(Query<R> query, CancellationToken ctx)
        {
            return await query.HandleAsync(_request, ctx);
        }

    }
}
