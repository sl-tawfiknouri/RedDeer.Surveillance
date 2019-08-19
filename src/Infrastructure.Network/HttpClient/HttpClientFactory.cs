namespace Infrastructure.Network.HttpClient
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Infrastructure.Network.HttpClient.Interfaces;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public class HttpClientFactory : IHttpClientFactory
    {
        private const string ApiAuthHeader = "authtoken";

        private readonly ILogger<HttpClientFactory> _logger;

        public HttpClientFactory(ILogger<HttpClientFactory> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public HttpClient ClientServiceHttpClient(string clientServiceUrl, string apiAccessToken)
        {
            if (string.IsNullOrWhiteSpace(clientServiceUrl))
            {
                this._logger.LogError(
                    $"{nameof(HttpClientFactory)} had a null or empty value for the {nameof(clientServiceUrl)}");
                throw new ArgumentOutOfRangeException(nameof(clientServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
                              {
                                  ServerCertificateCustomValidationCallback =
                                      (sender, certificate, chain, sslPolicyErrors) => true,
                                  UseCookies = true,
                                  CookieContainer = cookies,
                                  UseProxy = false
                              };

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(clientServiceUrl) };

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(ApiAuthHeader, apiAccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClientData = JsonConvert.SerializeObject(httpClient);
            this._logger.LogInformation($"http client created with details {httpClientData}");

            return httpClient;
        }

        public HttpClient GenericHttpClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                this._logger.LogError($"{nameof(HttpClientFactory)} had a null or empty value for the {nameof(url)}");
                throw new ArgumentOutOfRangeException(nameof(url));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
                              {
                                  ServerCertificateCustomValidationCallback =
                                      (sender, certificate, chain, sslPolicyErrors) => true,
                                  UseCookies = true,
                                  CookieContainer = cookies,
                                  UseProxy = false
                              };

            var httpClient = new HttpClient(handler) { BaseAddress = new Uri(url) };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClientData = JsonConvert.SerializeObject(httpClient);
            this._logger.LogInformation($"http client created with details {httpClientData}");

            return httpClient;
        }
    }
}