using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Configuration;
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
using Surveillance.Engine.Scheduler;
using Surveillance.Auditing.Context;
using Surveillance.Auditing.DataLayer.Processes;

namespace RedDeer.Surveillance.App
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
            SystemProcessContext.ProcessType = SystemProcessType.SurveillanceService;

            var container = new Container();
            
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var configBuilder = new Configuration.Configuration();
            var dbConfiguration = configBuilder.BuildDatabaseConfiguration(configurationBuilder);
            var apiConfiguration = configBuilder.BuildApiClientConfiguration(configurationBuilder);

            container.Inject(typeof(IDataLayerConfiguration), dbConfiguration);
            container.Inject(typeof(IAwsConfiguration), dbConfiguration);
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
                config.IncludeRegistry<RuleSchedulerRegistry>();
                config.IncludeRegistry<AppRegistry>();
                config.IncludeRegistry<RefinitivRegistry>();
                //config.Populate(services);
            });

            return container;
        }
    }
}
