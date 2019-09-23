namespace TestHarness.Repository.Api
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    using TestHarness.Configuration.Interfaces;

    public abstract class BaseApiRepository
    {
        private const string ApiAuthHeader = "authtoken";

        private readonly INetworkConfiguration _networkConfiguration;

        protected BaseApiRepository(INetworkConfiguration networkConfiguration)
        {
            this._networkConfiguration =
                networkConfiguration ?? throw new ArgumentNullException(nameof(networkConfiguration));
        }

        protected HttpClient BuildHttpClient()
        {
            if (string.IsNullOrWhiteSpace(this._networkConfiguration.ClientServiceUrl))
                throw new ArgumentOutOfRangeException(nameof(this._networkConfiguration.ClientServiceUrl));

            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
                              {
                                  ServerCertificateCustomValidationCallback =
                                      (sender, certificate, chain, sslPolicyErrors) => true,
                                  UseCookies = true,
                                  CookieContainer = cookies
                              };

            var httpClient = new HttpClient(handler)
                                 {
                                     BaseAddress = new Uri(this._networkConfiguration.ClientServiceUrl)
                                 };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                ApiAuthHeader,
                this._networkConfiguration.SurveillanceUserApiAccessToken);

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}