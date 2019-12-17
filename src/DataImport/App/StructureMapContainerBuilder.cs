using DataImport;
using DataImport.Configuration.Interfaces;
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
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using RedDeer.Extensions.Configuration.EC2Tags;
using Surveillance.Data.Universe.Refinitiv;

namespace RedDeer.DataImport.DataImport.App
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
            SystemProcessContext.ProcessType = SystemProcessType.DataImportService;

            var configurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEC2Tags(EC2TagsConstants.NestedSectionPath)
                .AddEnvironmentVariables()
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();

            var builtConfig = builder.Build(configurationRoot);
            var builtDataConfig = builder.BuildData(configurationRoot);
            var builtClientApiConfig = builder.BuildApi(configurationRoot);

            var container = new Container();
            container.Inject<IConfiguration>(configurationRoot);
            container.Inject(typeof(IUploadConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);            
            container.Inject(typeof(IAwsConfiguration), builtDataConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtDataConfig);
            container.Inject(typeof(IApiClientConfiguration), builtClientApiConfig);
            container.Inject(typeof(IRefinitivTickPriceHistoryApiConfig), builtConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataImportRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                config.IncludeRegistry<RefinitivRegistry>();
                // config.Populate(services);
            });

            return container;
        }
    }
}
