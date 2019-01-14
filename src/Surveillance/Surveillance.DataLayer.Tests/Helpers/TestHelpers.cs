using FakeItEasy;
using Surveillance.DataLayer.Configuration.Interfaces;

namespace Surveillance.DataLayer.Tests.Helpers
{
    public static class TestHelpers
    {
        public static string ConnectionString = "server=dev-temporary.cgedh3fdlw42.eu-west-1.rds.amazonaws.com; port=3306;uid=hackinguser;pwd='WillDelete3101';database=hackingdb1; Allow User Variables=True";
        public static string ClientServiceUrl = "http://localhost:8080";
        public static string ClientServiceAuthToken = "uwat";

        public static IDataLayerConfiguration Config()
        {
            var conf = A.Fake<IDataLayerConfiguration>();
            A.CallTo(() => conf.AuroraConnectionString).Returns(ConnectionString);
            A.CallTo(() => conf.ClientServiceUrl).Returns(ClientServiceUrl);
            A.CallTo(() => conf.SurveillanceUserApiAccessToken).Returns(ClientServiceAuthToken);

            return conf;
        }
    }
}
