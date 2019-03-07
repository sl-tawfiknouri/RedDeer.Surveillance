namespace Infrastructure.Network.HttpClient.Interfaces
{
    public interface IHttpClientFactory
    {
        System.Net.Http.HttpClient ClientServiceHttpClient(string clientServiceUrl, string apiAccessToken);
        System.Net.Http.HttpClient GenericHttpClient(string url);
    }
}