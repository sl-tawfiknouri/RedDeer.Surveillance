﻿using FakeItEasy;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

namespace Surveillance.Reddeer.ApiClient.Tests.Helpers
{
    public static class TestHelpers
    {
        public static string ConnectionString = "server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=dev_surveillance; Allow User Variables=True";
        public static string ClientServiceUrl = "http://localhost:8080";
        public static string ClientServiceAuthToken = "uwat";

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
