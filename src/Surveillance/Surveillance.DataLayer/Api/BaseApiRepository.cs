using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Api
{
    public abstract class BaseApiRepository
    {
        private const string ApiAuthHeader = "authtoken";
        private readonly IDataLayerConfiguration _dataLayerConfiguration;
        private readonly ILogger _logger;

        protected BaseApiRepository(
            IDataLayerConfiguration dataLayerConfiguration,
            ILogger logger)
        {
            _dataLayerConfiguration =
                dataLayerConfiguration
                ?? throw new ArgumentNullException(nameof(dataLayerConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected HttpClient BuildHttpClient()
        {
            if (string.IsNullOrWhiteSpace(_dataLayerConfiguration.ClientServiceUrl))
            {
                _logger.LogError($"BaseApiRepository had a null or empty value for the client service url");
                throw new ArgumentOutOfRangeException(nameof(_dataLayerConfiguration.ClientServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies,
                UseProxy = false
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

        protected HttpClient BuildBmllHttpClient()
        {
            if (string.IsNullOrWhiteSpace(_dataLayerConfiguration.BmllServiceUrl))
            {
                _logger.LogError($"BaseApiRepository had a null or empty value for the bmll service url");
                throw new ArgumentOutOfRangeException(nameof(_dataLayerConfiguration.BmllServiceUrl));
            }

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true,
                UseCookies = true,
                CookieContainer = cookies,
                UseProxy = false
            };

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_dataLayerConfiguration.BmllServiceUrl)
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
