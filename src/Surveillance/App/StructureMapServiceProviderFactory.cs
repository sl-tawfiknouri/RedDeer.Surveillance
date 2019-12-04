using System;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing;
using Surveillance.DataLayer;
using Surveillance;
using Surveillance.Engine.RuleDistributor;
using Surveillance.Engine.DataCoordinator;
using Surveillance.Engine.Rules;
using Surveillance.Reddeer.ApiClient;
using Surveillance.Data.Universe.Refinitiv;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;

namespace RedDeer.Surveillance.App
{
    public class StructureMapServiceProviderFactory : IServiceProviderFactory<Container>, IDisposable
    {
        private Container container = new Container();

        public Container CreateBuilder(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .Build();

            var configBuilder = new Configuration.Configuration();
            var dbConfiguration = configBuilder.BuildDatabaseConfiguration(configurationBuilder);

            var container = new Container();
            container.Inject(typeof(IDataLayerConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);

            var apiConfiguration = configBuilder.BuildApiClientConfiguration(configurationBuilder);
            container.Inject(typeof(IApiClientConfiguration), apiConfiguration);

            container.Inject(typeof(IRuleConfiguration), configBuilder.BuildRuleConfiguration(configurationBuilder));
            container.Inject(typeof(ISystemDataLayerConfig), configBuilder.BuildDataLayerConfig(configurationBuilder));
            container.Inject(typeof(IRefinitivTickPriceHistoryApiConfig), configBuilder.BuildRefinitivTickPriceHistoryApiConfig(configurationBuilder));

            container.Configure(config =>
            {
                config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                config.IncludeRegistry<DataLayerRegistry>();
                config.IncludeRegistry<SurveillanceRegistry>();
                config.IncludeRegistry<RuleDistributorRegistry>();
                config.IncludeRegistry<DataCoordinatorRegistry>();
                config.IncludeRegistry<RuleRegistry>();
                config.IncludeRegistry<ReddeerApiClientRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<RefinitivRegistry>();
                config.Populate(services);
            });

            return container;
        }

        public IServiceProvider CreateServiceProvider(Container container)
        {
            container.GetInstance<IScriptRunner>();
            return container.GetInstance<IServiceProvider>();
        }

        /// <summary>
        /// Container must be dispose, if not dispose, Host service can't release the process.
        /// </summary>
        public void Dispose()
            => container?.Dispose();
    }
}