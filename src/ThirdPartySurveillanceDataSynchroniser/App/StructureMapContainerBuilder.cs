using DataSynchroniser.Api.Bmll;
using DataSynchroniser.Api.Factset;
using DataSynchroniser.Api.Markit;
using DataSynchroniser.Api.Refinitive;
using DataSynchroniser.Configuration;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Configuration;
using RedDeer.Extensions.Configuration.EC2Tags;
using StructureMap;
using Surveillance.Auditing;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Auditing.DataLayer.Processes;
using Surveillance.Data.Universe.Refinitiv;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
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

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEC2Tags(EC2TagsConstants.NestedSectionPath)
                .AddEnvironmentVariables()
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            var builtConfig = builder.Build(configurationRoot);
            
            var container = new Container();
            container.Inject<IConfiguration>(configurationRoot);
            container.Inject(typeof(IAwsConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);
            container.Inject(typeof(IApiClientConfiguration), builtConfig);
            container.Inject(typeof(IRefinitivTickPriceHistoryApiConfig), builtConfig);

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
                config.IncludeRegistry<RefinitivDataSynchroniserRegistry>();
                config.IncludeRegistry<RefinitivRegistry>();
                // config.Populate(services);
            });

            return container;
        }
    }
}
