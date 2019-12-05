using DataSynchroniser.Api.Bmll;
using DataSynchroniser.Api.Factset;
using DataSynchroniser.Api.Markit;
using DataSynchroniser.Configuration;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Configuration;
using StructureMap;
using Surveillance.Auditing;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;

namespace DataSynchroniser.App
{
    public static class StructureMapContainer
    {
        private static Container _instance = null;
        private static readonly object @lock = new object();

        public static Container Instance
        {
            get
            {
                lock (@lock)
                {
                    if (_instance == null)
                    {
                        _instance = new StructureMapContainerBuilder().Build();
                    }

                    return _instance;
                }
            }
        }
    }

    public class StructureMapContainerBuilder
    {
        public Container Build()
        {
            SystemProcessContext.ProcessType = SystemProcessType.ThirdPartySurveillanceDataSynchroniser;

            var builtConfig = BuildConfiguration();
            
            var container = new Container();
            container.Inject(typeof(IAwsConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);
            container.Inject(typeof(IApiClientConfiguration), builtConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<DataSynchroniserRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<BmllDataSynchroniserRegistry>();
                config.IncludeRegistry<FactsetDataSynchroniserRegistry>();
                config.IncludeRegistry<MarkitDataSynchroniserRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                // config.Populate(services);
            });

            return container;
        }

        private static Config BuildConfiguration()
        {
            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.Build(configurationRoot);
        }
    }
}
