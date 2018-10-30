using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TestHarness.Configuration.Interfaces;

namespace TestHarness.Repository.Api
{
    public abstract class BaseApiRepository
    {
        private const string ApiAuthHeader = "authtoken";
        private readonly INetworkConfiguration _networkConfiguration;

        protected BaseApiRepository(INetworkConfiguration networkConfiguration)
        {
            _networkConfiguration =
                networkConfiguration
                ?? throw new ArgumentNullException(nameof(networkConfiguration));
        }

        protected HttpClient BuildHttpClient()
        {
            if (string.IsNullOrWhiteSpace(_networkConfiguration.ClientServiceUrl))
            {
                throw new ArgumentOutOfRangeException(nameof(_networkConfiguration.ClientServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_networkConfiguration.ClientServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(ApiAuthHeader, _networkConfiguration.SurveillanceUserApiAccessToken);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
