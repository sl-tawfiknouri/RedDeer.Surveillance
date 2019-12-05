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
                .AddEnvironmentVariables()
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();

            var builtConfig = builder.Build(configurationRoot);
            var builtDataConfig = builder.BuildData(configurationRoot);
            var builtClientApiConfig = builder.BuildApi(configurationRoot);

            var container = new Container();
            container.Inject(typeof(IUploadConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);            
            container.Inject(typeof(IAwsConfiguration), builtDataConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtDataConfig);
            container.Inject(typeof(IApiClientConfiguration), builtClientApiConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataImportRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                // config.Populate(services);
            });

            return container;
        }
    }
}
