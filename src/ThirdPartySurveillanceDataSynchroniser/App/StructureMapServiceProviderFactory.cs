using DataSynchroniser.Api.Bmll;
using DataSynchroniser.Api.Factset;
using DataSynchroniser.Api.Markit;
using DataSynchroniser.Configuration;
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
using System;

namespace DataSynchroniser.App
{
    public class StructureMapServiceProviderFactory : IServiceProviderFactory<Container>, IDisposable
    {
        private Container container = new Container();

        public Container CreateBuilder(IServiceCollection services)
        {
            var builtConfig = BuildConfiguration();
            container.Inject(typeof(IAwsConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);

            container.Inject(typeof(IAwsConfiguration), builtConfig);
            container.Inject(typeof(ISystemDataLayerConfig), builtConfig);
            container.Inject(typeof(IDataLayerConfiguration), builtConfig);

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
                config.Populate(services);
            });

            SystemProcessContext.ProcessType = SystemProcessType.ThirdPartySurveillanceDataSynchroniser;

            return container;
        }

        public IServiceProvider CreateServiceProvider(Container container)
            => container?.GetInstance<IServiceProvider>();

        /// <summary>
        /// Container must be dispose, if not dispose, Host service can't release the process.
        /// </summary>
        public void Dispose()
            => container?.Dispose();

        private static Config BuildConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var builder = new ConfigBuilder.ConfigBuilder();
            return builder.Build(configurationBuilder);
        }
    }
}
