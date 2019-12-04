using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using System;

namespace RedDeer.DataImport.DataImport.App
{
    public class StructureMapServiceProviderFactory : IServiceProviderFactory<Container>, IDisposable
    {
        private Container container = new Container();

        public Container CreateBuilder(IServiceCollection services)
        {
            var builtConfig = BuildConfiguration();
            container.Inject(typeof(IUploadConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            SystemProcessContext.ProcessType = SystemProcessType.DataImportService;

            var builtDataConfig = BuildDataConfiguration();
            container.Inject(typeof(IAwsConfiguration), builtDataConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtDataConfig);

            var builtClientApiConfig = BuildApiClientConfiguration();
            container.Inject(typeof(IApiClientConfiguration), builtClientApiConfig);

            container.Configure(config =>
            {
                config.IncludeRegistry<DataImportRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                config.Populate(services);
            });

            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);

            return container;
        }

        public IServiceProvider CreateServiceProvider(Container container)
            => container.GetInstance<IServiceProvider>();

        private static IApiClientConfiguration BuildApiClientConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.BuildApi(configurationBuilder);
        }

        private static Configuration BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.Build(configurationBuilder);
        }

        private static IDataLayerConfiguration BuildDataConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.BuildData(configurationBuilder);
        }

        public void Dispose()
            => container?.Dispose();
    }
}
