namespace Surveillance.Reddeer.ApiClient.Tests.Helpers
{
    using FakeItEasy;

    using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

    /// <summary>
    /// The test helpers.
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// The client service auth token.
        /// </summary>
        public static string ClientServiceAuthToken = "uwat";

        /// <summary>
        /// The client service url.
        /// </summary>
        public static string ClientServiceUrl = "http://localhost:8080";

        /// <summary>
        /// The connection string.
        /// </summary>
        public static string ConnectionString =
            "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True";

        /// <summary>
        /// The config.
        /// </summary>
        /// <returns>
        /// The <see cref="IApiClientConfiguration"/>.
        /// </returns>
        public static IApiClientConfiguration Config()
        {
            var conf = A.Fake<IApiClientConfiguration>();
            A.CallTo(() => conf.AuroraConnectionString).Returns(ConnectionString);
            A.CallTo(() => conf.ClientServiceUrl).Returns(ClientServiceUrl);
            A.CallTo(() => conf.SurveillanceUserApiAccessToken).Returns(ClientServiceAuthToken);

            return conf;
        }
    }
}