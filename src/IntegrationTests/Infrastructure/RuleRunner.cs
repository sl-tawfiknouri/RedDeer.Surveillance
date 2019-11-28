using Dapper;
using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using RedDeer.Contracts.SurveillanceService.Rules;
using RedDeer.Surveillance.App;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using StructureMap;
using Surveillance.Api.DataAccess.Abstractions.DbContexts;
using Surveillance.Api.DataAccess.DbContexts.Factory;
using Surveillance.Auditing;
using Surveillance.Auditing.DataLayer;
using Surveillance.Auditing.DataLayer.Interfaces;
using Surveillance.Data.Universe.Refinitiv;
using Surveillance.Data.Universe.Refinitiv.Interfaces;
using Surveillance.DataLayer;
using Surveillance.DataLayer.Configuration;
using Surveillance.DataLayer.Configuration.Interfaces;
using Surveillance.Engine.DataCoordinator;
using Surveillance.Engine.DataCoordinator.Coordinator.Interfaces;
using Surveillance.Engine.Rules;
using Surveillance.Engine.Rules.Queues.Interfaces;
using Surveillance.Engine.Rules.Utility.Interfaces;
using Surveillance.Reddeer.ApiClient;
using Surveillance.Reddeer.ApiClient.Configuration;
using Surveillance.Reddeer.ApiClient.Configuration.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.IntegrationTests.Infrastructure
{
    public class RuleRunner
    {
        public static readonly string DatabaseConfig = @"server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=test_surveillance; Allow User Variables=True";

        public static readonly string CorrelationId = "testCorrelation";
        public static readonly string RuleId = "testRule";
        public static readonly string MessageId = "testMessage";

        public WashTradeRuleParameterDto WashTradeParameters { get; set; }

        public string TradeCsvContent { get; set; }
        public string AllocationCsvContent { get; set; }

        public int ExpectedOrderCount { get; set; } = 0;
        public int ExpectedAllocationCount { get; set; } = 0;

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<RuleBreachWithOrders> OriginalRuleBreaches { get; set; }
        public List<RuleBreachWithOrders> RemainingRuleBreaches { get; set; }
        public bool HasCheckedForNoBreaches { get; set; }

        private ILogger<RuleRunner> _logger;

        public async Task Run()
        {
            SetupLogger();
            await SetupDatabase();
            ImportAllocationsAndTrades();

            using (var dbContext = BuildDbContext())
            {
                var orderCount = GetOrderCount(dbContext);
                orderCount.Should().Be(ExpectedOrderCount);

                var allocationCount = GetOrderAllocationCount(dbContext);
                allocationCount.Should().Be(ExpectedAllocationCount);

                await RunRule();

                OriginalRuleBreaches = GetRuleBreaches(dbContext);
                RemainingRuleBreaches = OriginalRuleBreaches.ToList();
            }

            true.Should().Be(true);
        }

        private void SetupLogger()
        {
            var container = new Container();

            var nLogConfig = new LoggingConfiguration();
            var logconsole = new ConsoleTarget("logconsole");
            nLogConfig.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Fatal, logconsole);
            LogManager.Configuration = nLogConfig; // set config when nlog used from LogManager.GetCurrentClassLogger()
            NLogBuilder.ConfigureNLog(nLogConfig); // set config when nlog used from asp net core

            container.Configure(config => config.IncludeRegistry<NLogRegistry>());

            _logger = container.GetInstance<ILogger<RuleRunner>>();
        }

        private class NLogRegistry : Registry
        {
            public NLogRegistry()
            {
                var loggerFactory = new NLogLoggerFactory();
                this.For(typeof(ILoggerFactory)).Use(loggerFactory);
                this.For(typeof(ILogger<>)).Use(typeof(Logger<>));
            }
        }

        private async Task SetupDatabase()
        {
            var container = new Container();

            container.Configure(
                    config =>
                    {
                        config.IncludeRegistry<AppRegistry>();
                        config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                    });

            var systemDataLayerConfig = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = DatabaseConfig,
                OverrideMigrationsFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Migrations")
            };
            container.Inject(typeof(ISystemDataLayerConfig), systemDataLayerConfig);

            _logger.LogInformation($"Migrations folder: {systemDataLayerConfig.OverrideMigrationsFolder}");

            // IScriptRunner constructor will run database migrations
            container.GetInstance<IScriptRunner>();

            await EmptyTables(container);
        }

        private async Task EmptyTables(Container container)
        {
            var script = @"
                truncate fileuploadorders;
                truncate fileuploadallocations;
                truncate systemprocessoperationuploadfile;
                delete from dealerorders where Id > 0;
                delete from orders where Id > 0;
                truncate ordersallocation;
                delete from instrumentequitytimebars where Id > 0;
                delete from instrumentequitydailysummary where Id > 0;
                delete from ruledatarequest where Id > 0;
                delete from financialinstruments where Id > 0;
                delete from market where Id > 0;
                truncate rulebreach;
                truncate rulebreachorders;
            ";

            var factory = container.GetInstance<IConnectionStringFactory>();

            using (var dbConnection = factory.BuildConn())
            using (var conn = dbConnection.ExecuteAsync(script))
            {
                await conn;
                _logger.LogInformation($"Emptied tables");
            }
        }

        private void ImportAllocationsAndTrades()
        {
            var container = new Container();

            // registries
            container.Configure(
                    config =>
                    {
                        config.IncludeRegistry<DataImportRegistry>();
                        config.IncludeRegistry<DataLayerRegistry>();
                        config.IncludeRegistry<ReddeerApiClientRegistry>();
                        config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                        config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                        config.IncludeRegistry<DataCoordinatorRegistry>();
                    });

            // configuration
            var dataImportConfiguration = new Configuration();
            container.Inject(typeof(IUploadConfiguration), dataImportConfiguration);

            var dataLayerConfiguration = new DataLayerConfiguration
            {
                AuroraConnectionString = DatabaseConfig
            };
            container.Inject(typeof(IDataLayerConfiguration), dataLayerConfiguration);

            var apiClientConfiguration = new ApiClientConfiguration();
            container.Inject(typeof(IApiClientConfiguration), apiClientConfiguration);
            container.Inject(typeof(IAwsConfiguration), apiClientConfiguration);

            var systemDataLayerConfig = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = DatabaseConfig
            };
            container.Inject(typeof(ISystemDataLayerConfig), systemDataLayerConfig);

            // replace aws queue client with fake
            var awsQueueClient = A.Fake<IAwsQueueClient>();
            container.Inject(typeof(IAwsQueueClient), awsQueueClient);

            // upload allocation file
            if (AllocationCsvContent != null)
            {
                var uploadAllocationFileMonitor = container.GetInstance<IUploadAllocationFileMonitor>();
                WithTempFile(AllocationCsvContent, fileName => uploadAllocationFileMonitor.ProcessFile(fileName));
            }

            // upload trade file
            if (TradeCsvContent != null)
            {
                var uploadTradeFileMonitor = container.GetInstance<IUploadTradeFileMonitor>();
                WithTempFile(TradeCsvContent, fileName => uploadTradeFileMonitor.ProcessFile(fileName));
            }

            // enliven orders with data verifier
            var dataVerifier = container.GetInstance<IDataVerifier>();
            dataVerifier.Scan();
        }

        private void WithTempFile(string content, Action<string> action)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), DateTime.UtcNow.ToFileTime().ToString());
            var tempFile = Path.Combine(tempFolder, "file");
            try
            {
                Directory.CreateDirectory(tempFolder);
                File.WriteAllText(tempFile, content);

                _logger.LogInformation($"Running action WithTempFile {tempFile}");
                action(tempFile);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }

        private async Task RunRule()
        {
            var container = new Container();

            // registries
            container.Configure(
                    config =>
                    {
                        config.IncludeRegistry<RuleRegistry>();
                        config.IncludeRegistry<DataLayerRegistry>();
                        config.IncludeRegistry<ReddeerApiClientRegistry>();
                        config.IncludeRegistry<RefinitivRegistry>();
                        config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                        config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                    });

            // configuration
            var dataLayerConfiguration = new DataLayerConfiguration
            {
                AuroraConnectionString = DatabaseConfig
            };
            container.Inject(typeof(IDataLayerConfiguration), dataLayerConfiguration);

            var apiClientConfiguration = new ApiClientConfiguration();
            container.Inject(typeof(IApiClientConfiguration), apiClientConfiguration);
            container.Inject(typeof(IAwsConfiguration), apiClientConfiguration);

            var refinitivTickPriceHistoryApiConfig = new RefinitivTickPriceHistoryApiConfig();
            container.Inject(typeof(IRefinitivTickPriceHistoryApiConfig), refinitivTickPriceHistoryApiConfig);

            var systemDataLayerConfig = new SystemDataLayerConfig
            {
                SurveillanceAuroraConnectionString = DatabaseConfig
            };
            container.Inject(typeof(ISystemDataLayerConfig), systemDataLayerConfig);

            // replace aws queue client with fake
            var awsQueueClient = A.Fake<IAwsQueueClient>();
            container.Inject(typeof(IAwsQueueClient), awsQueueClient);

            // replace api heartbeat
            var apiHeartbeat = A.Fake<IApiHeartbeat>();
            A.CallTo(() => apiHeartbeat.HeartsBeating()).Returns(Task.FromResult(true));
            container.Inject(typeof(IApiHeartbeat), apiHeartbeat);

            // replace rule parameter api
            var ruleParameterApi = A.Fake<IRuleParameterApi>();
            A.CallTo(() => ruleParameterApi.GetAsync(RuleId))
                .ReturnsLazily(() =>
                {
                    var dto = new RuleParameterDto
                    {
                        CancelledOrders = new CancelledOrderRuleParameterDto[0],
                        HighProfits = new HighProfitsRuleParameterDto[0],
                        MarkingTheCloses = new MarkingTheCloseRuleParameterDto[0],
                        Spoofings = new SpoofingRuleParameterDto[0],
                        Layerings = new LayeringRuleParameterDto[0],
                        HighVolumes = new HighVolumeRuleParameterDto[0],
                        WashTrades = WashTradeParameters.CreateArray(),
                        Rampings = new RampingRuleParameterDto[0],
                        PlacingOrders = new PlacingOrdersWithNoIntentToExecuteRuleParameterDto[0],
                        FixedIncomeHighVolumeIssuance = new FixedIncomeHighVolumeRuleParameterDto[0],
                        FixedIncomeHighProfits = new FixedIncomeHighProfitRuleParameterDto[0],
                        FixedIncomeWashTrades = new FixedIncomeWashTradeRuleParameterDto[0]
                    };
                    return Task.FromResult(dto);
                });
            container.Inject(typeof(IRuleParameterApi), ruleParameterApi);

            // replace market open close api
            var marketOpenCloseApi = A.Fake<IMarketOpenCloseApi>();
            container.Inject(typeof(IMarketOpenCloseApi), marketOpenCloseApi);

            // execution message
            var execution = new ScheduledExecution
            {
                TimeSeriesInitiation = new DateTimeOffset(From),
                TimeSeriesTermination = new DateTimeOffset(To),
                CorrelationId = CorrelationId,
                IsBackTest = true,
                Rules = new List<RuleIdentifier>
                {
                    new RuleIdentifier
                    {
                        Rule = GetRuleType(),
                        Ids = new[] { RuleId }
                    }
                }
            };
            var message = JsonConvert.SerializeObject(execution);

            // run rule
            var queueRuleSubscriber = container.GetInstance<IQueueRuleSubscriber>();
            await queueRuleSubscriber.ExecuteDistributedMessage(MessageId, message);
        }

        private Rules GetRuleType()
        {
            if (WashTradeParameters != null)
            {
                return Rules.WashTrade;
            }

            return Rules.Spoofing;
        }

        private int GetOrderCount(IGraphQlDbContext dbContext)
        {
            return dbContext
                .Orders
                .Count();
        }

        private int GetOrderAllocationCount(IGraphQlDbContext dbContext)
        {
            return dbContext
                .OrdersAllocation
                .Count();
        }

        private List<RuleBreachWithOrders> GetRuleBreaches(IGraphQlDbContext dbContext)
        {
            var ruleBreaches = dbContext
                .RuleBreach
                .Where(x => x.CorrelationId == CorrelationId)
                .AsNoTracking()
                .ToList();

            var breachIds = ruleBreaches
                .Select(x => x.Id);

            var ruleBreachOrders = dbContext
                .RuleBreachOrders
                .Where(x => breachIds.Contains(x.RuleBreachId))
                .AsNoTracking()
                .ToList();

            var orderIds = ruleBreachOrders
                .Select(x => x.OrderId);

            var orders = dbContext
                .Orders
                .Where(x => orderIds.Contains(x.Id))
                .AsNoTracking()
                .ToList();

            var mappedBreaches = ruleBreaches
                .Select(x => new RuleBreachWithOrders
                {
                    RuleBreach = x,
                    Orders = ruleBreachOrders
                        .Where(y => y.RuleBreachId == x.Id)
                        .Select(y => orders.Single(z => z.Id == y.OrderId))
                        .ToList()
                })
                .ToList();

            return mappedBreaches;
        }

        private IGraphQlDbContext BuildDbContext()
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["SurveillanceApiConnectionString"] = DatabaseConfig
            });
            var configuration = builder.Build();

            var factory = new GraphQlDbContextFactory(configuration, new NLogLoggerFactory());

            return factory.Build();
        }

        public void ErrorOnUnaccountedRuleBreaches()
        {
            if (RemainingRuleBreaches?.Any() ?? false)
            {
                throw new Exception($"There are {RemainingRuleBreaches.Count} unaccounted for rule breaches remaining");
            }
        }

        public void ErrorIfUncheckedForNoBreaches()
        {
            if (OriginalRuleBreaches != null && OriginalRuleBreaches.Count == 0 && !HasCheckedForNoBreaches)
            {
                throw new Exception($"There was no specific assertion that 0 rules breaches were found");
            }
        }
    }
}
