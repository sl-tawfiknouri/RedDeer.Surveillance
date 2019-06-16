using System.Threading;
using Microsoft.Extensions.Configuration;
using NLog;

namespace TestHarness.App
{
    public class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            var networkConfiguration = BuildConfiguration();

            LogManager.LoadConfiguration("nlog.config");
            Bootstrapper.Start(networkConfiguration);

            Thread.Sleep(50);
        }

        private static Configuration.Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var config = ConfigurationHelper.BuildNetworkConfiguration(configurationBuilder);

            return config;
        }
    }
}