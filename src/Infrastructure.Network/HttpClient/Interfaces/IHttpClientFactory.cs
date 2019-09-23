namespace Infrastructure.Network.HttpClient.Interfaces
{
    using System.Net.Http;

    public interface IHttpClientFactory
    {
        HttpClient ClientServiceHttpClient(string clientServiceUrl, string apiAccessToken);

        HttpClient GenericHttpClient(string url);
    }
}