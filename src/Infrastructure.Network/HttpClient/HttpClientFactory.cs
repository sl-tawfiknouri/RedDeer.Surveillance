using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Infrastructure.Network.HttpClient.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Network.HttpClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private const string ApiAuthHeader = "authtoken";
        private readonly ILogger<HttpClientFactory> _logger;

        public HttpClientFactory(ILogger<HttpClientFactory> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public System.Net.Http.HttpClient ClientServiceHttpClient(string clientServiceUrl, string apiAccessToken)
        {
            if (string.IsNullOrWhiteSpace(clientServiceUrl))
            {
                _logger.LogError($"{nameof(HttpClientFactory)} had a null or empty value for the {nameof(clientServiceUrl)}");
                throw new ArgumentOutOfRangeException(nameof(clientServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies,
                UseProxy = false
            };

            var httpClient = new System.Net.Http.HttpClient(handler)
            {
                BaseAddress = new Uri(clientServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(ApiAuthHeader, apiAccessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClientData = JsonConvert.SerializeObject(httpClient);
            _logger.LogInformation($"http client created with details {httpClientData}");

            return httpClient;
        }

        public System.Net.Http.HttpClient GenericHttpClient(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.LogError($"{nameof(HttpClientFactory)} had a null or empty value for the {nameof(url)}");
                throw new ArgumentOutOfRangeException(nameof(url));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies,
                UseProxy = false
            };

            var httpClient = new System.Net.Http.HttpClient(handler)
            {
                BaseAddress = new Uri(url)
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpClientData = JsonConvert.SerializeObject(httpClient);
            _logger.LogInformation($"http client created with details {httpClientData}");

            return httpClient;
        }
    }
}
