using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api
{
    public abstract class BaseApiRepository
    {
        private const string ApiAuthHeader = "authtoken";
        private readonly IDataLayerConfiguration _dataLayerConfiguration;

        protected BaseApiRepository(IDataLayerConfiguration dataLayerConfiguration)
        {
            _dataLayerConfiguration =
                dataLayerConfiguration
                ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
        }

        protected HttpClient BuildHttpClient()
        {
            if (string.IsNullOrWhiteSpace(_dataLayerConfiguration.ClientServiceUrl))
            {
                throw new ArgumentOutOfRangeException(nameof(_dataLayerConfiguration.ClientServiceUrl));
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
                BaseAddress = new Uri(_dataLayerConfiguration.ClientServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(ApiAuthHeader, _dataLayerConfiguration.SurveillanceUserApiAccessToken);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
