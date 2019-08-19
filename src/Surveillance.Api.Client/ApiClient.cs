namespace RedDeer.Surveillance.Api.Client
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using RedDeer.Surveillance.Api.Client.Infrastructure;

    public class ApiClient
    {
        private readonly Request _request;

        public ApiClient(string url, string bearer, HttpMessageHandler httpMessageHandler = null)
        {
            this._request = new Request(url, bearer, httpMessageHandler);
        }

        public async Task<R> QueryAsync<R>(Query<R> query, CancellationToken ctx)
        {
            return await query.HandleAsync(this._request, ctx);
        }
    }
}