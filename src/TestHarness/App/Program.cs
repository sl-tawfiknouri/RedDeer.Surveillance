namespace TestHarness.App
{
    using System.Threading;

    using Microsoft.Extensions.Configuration;

    using NLog;

    using TestHarness.Configuration;

    public class Program
    {
        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var config = ConfigurationHelper.BuildNetworkConfiguration(configurationBuilder);

            return config;
        }

        // ReSharper disable once UnusedParameter.Local
        private static void Main(string[] args)
        {
            var networkConfiguration = BuildConfiguration();

            LogManager.LoadConfiguration("nlog.config");
            Bootstrapper.Start(networkConfiguration);

            Thread.Sleep(50);
        }
    }
}