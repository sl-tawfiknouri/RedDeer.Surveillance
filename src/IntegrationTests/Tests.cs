using Dapper;
using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using FakeItEasy;
using FluentAssertions;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using RedDeer.Contracts.SurveillanceService.Rules;
using RedDeer.Surveillance.App;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using StructureMap;
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
using System.Reflection;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.IntegrationTests
{
    public class Tests
    {
        static readonly string DatabaseConfig = @"server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=test_surveillance; Allow User Variables=True";

        private ILogger<Tests> _logger;

        [SetUp]
        public async Task SetUp()
        {
            SetupLogger();
            await SetupDatabase();
        }

        [Test]
        public async Task MyTest()
        {
            var trades = @"MarketIdentifierCode,MarketName,MarketType,InstrumentName,InstrumentCfi,InstrumentIssuerIdentifier,InstrumentClientIdentifier,InstrumentSedol,InstrumentIsin,InstrumentFigi,InstrumentCusip,InstrumentLei,InstrumentExchangeSymbol,InstrumentBloombergTicker,InstrumentUnderlyingName,InstrumentUnderlyingCfi,InstrumentUnderlyingIssuerIdentifier,InstrumentUnderlyingClientIdentifier,InstrumentUnderlyingSedol,InstrumentUnderlyingIsin,InstrumentUnderlyingFigi,InstrumentUnderlyingCusip,InstrumentUnderlyingLei,InstrumentUnderlyingExchangeSymbol,InstrumentUnderlyingBloombergTicker,OrderId,OrderVersion,OrderVersionLinkId,OrderGroupId,OrderPlacedDate,OrderBookedDate,OrderAmendedDate,OrderRejectedDate,OrderCancelledDate,OrderFilledDate,OrderType,OrderDirection,OrderCurrency,OrderSettlementCurrency,OrderCleanDirty,OrderAccumulatedInterest,OrderLimitPrice,OrderAverageFillPrice,OrderOrderedVolume,OrderFilledVolume,OrderTraderId,OrderTraderName,OrderClearingAgent,OrderDealingInstructions,OrderOptionStrikePrice,OrderOptionExpirationDate,OrderOptionEuropeanAmerican,DealerOrderId,DealerOrderVersion,DealerOrderVersionLinkId,DealerOrderGroupId,DealerOrderPlacedDate,DealerOrderBookedDate,DealerOrderAmendedDate,DealerOrderRejectedDate,DealerOrderCancelledDate,DealerOrderFilledDate,DealerOrderDealerId,DealerOrderDealerName,DealerOrderNotes,DealerOrderCounterParty,DealerOrderType,DealerOrderDirection,DealerOrderCurrency,DealerOrderSettlementCurrency,DealerOrderCleanDirty,DealerOrderAccumulatedInterest,DealerOrderLimitPrice,DealerOrderAverageFillPrice,DealerOrderOrderedVolume,DealerOrderFilledVolume,DealerOrderOptionStrikePrice,DealerOrderOptionExpirationDate,DealerOrderOptionEuropeanAmerican
XLON,London Stock Exchange,STOCKEXCHANGE,VODAFONE,entpsb,Vodafone Corp,VOD LN,BH4HKS3,GBOOBH4HKS39,BBG000C3K3G9,G93882192,,VOD,VOD LN Equity,,,,,,,,,,,,420cwashtradelgim,,,ABCD-EFGH,2018-01-02T15:00:00,2018-01-02T15:00:00,2018-01-02T15:00:00,,,2018-01-02T15:00:00,MARKET,BUY,GBP,GBP,CLEAN,,1.5,3,1000,1000,Fahads1-125,Fahads1,ClearingAgent,Trade within 1% of VWAP,0,2018-01-02T15:00:00,EUROPEAN,1111,1,1,2,2018-01-02T15:00:00,2018-01-02T15:00:00,2018-01-02T15:00:00,,,2018-01-02T15:00:00,Fahads1-111,Fahads1 Dealers,some orders,Goldman Sachs,MARKET,BUY,GBP,GBP,CLEAN,0,1.5,2,1000,1000,0,12/12/2018 15:30,EUROPEAN
XLON,London Stock Exchange,STOCKEXCHANGE,VODAFONE,entpsb,Vodafone Corp,VOD LN,BH4HKS3,GBOOBH4HKS39,BBG000C3K3G9,G93882192,,VOD,VOD LN Equity,,,,,,,,,,,,421cwashtradelgim,,,ABCD-EFGH,2018-01-02T15:00:00,2018-01-02T15:00:00,2018-01-02T15:00:00,,,2018-01-02T15:00:00,MARKET,SELL,GBP,GBP,CLEAN,,1.5,3,1000,1000,Fahads1-125,Fahads1,ClearingAgent,Trade within 1% of VWAP,0,2018-01-02T15:00:00,EUROPEAN,1112,1,1,2,2018-01-02T15:00:00,2018-01-02T15:00:00,2018-01-02T15:00:00,,,2018-01-02T15:00:00,Fahads1-111,Fahads1 Dealers,some orders,Goldman Sachs,MARKET,SELL,GBP,GBP,CLEAN,0,1.5,10,1000,1000,0,12/12/2018 15:30,EUROPEAN";

            var allocations = @"OrderId,Fund,Strategy,ClientAccountId,OrderFilledVolume
420cwashtradelgim,FundE,JapanE,111,10000000000
421cwashtradelgim,FundE,JapanE,111,10000000000";

            ImportAllocationsAndTrades(allocations, trades);

            await RunRule();

            true.Should().Be(true);
        }

        private void SetupLogger()
        {
            var container = new Container();

            var nLogConfig = new LoggingConfiguration();
            var logconsole = new ConsoleTarget("logconsole");
            nLogConfig.AddRule(NLog.LogLevel.Error, NLog.LogLevel.Fatal, logconsole);
            LogManager.Configuration = nLogConfig; // set config when nlog used from LogManager.GetCurrentClassLogger()
            NLogBuilder.ConfigureNLog(nLogConfig); // set config when nlog used from asp net core

            container.Configure(config => config.IncludeRegistry<NLogRegistry>());

            _logger = container.GetInstance<ILogger<Tests>>();
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

        private void ImportAllocationsAndTrades(string allocations, string trades)
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
            var uploadAllocationFileMonitor = container.GetInstance<IUploadAllocationFileMonitor>();
            WithTempFile(allocations, fileName => uploadAllocationFileMonitor.ProcessFile(fileName));

            // upload trade file
            var uploadTradeFileMonitor = container.GetInstance<IUploadTradeFileMonitor>();
            WithTempFile(trades, fileName => uploadTradeFileMonitor.ProcessFile(fileName));

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
            A.CallTo(() => ruleParameterApi.GetAsync("rule123"))
                .ReturnsLazily(() =>
                {
                    var dto = BlankRuleParameterDto();
                    dto.WashTrades = new WashTradeRuleParameterDto[]
                    {
                        new WashTradeRuleParameterDto
                        {
                            Id = "rule123",
                            WindowSize = TimeSpan.FromDays(1),
                            PerformClusteringPositionAnalysis = true,
                            ClusteringPercentageValueDifferenceThreshold = 0.010M,
                            ClusteringPositionMinimumNumberOfTrades = 2,
                            OrganisationalFactors = new[] { OrganisationalFactors.None }
                        }
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
                TimeSeriesInitiation = new DateTimeOffset(new DateTime(2018, 01, 01, 00, 00, 00)),
                TimeSeriesTermination = new DateTimeOffset(new DateTime(2018, 01, 03, 00, 00, 00)),
                CorrelationId = "correlation123",
                IsBackTest = true,
                Rules = new List<RuleIdentifier>
                {
                    new RuleIdentifier
                    {
                        Rule = Rules.WashTrade,
                        Ids = new[] { "rule123" }
                    }
                }
            };
            var message = JsonConvert.SerializeObject(execution);

            // run rule
            var queueRuleSubscriber = container.GetInstance<IQueueRuleSubscriber>();
            await queueRuleSubscriber.ExecuteDistributedMessage("message123", message);
        }

        private RuleParameterDto BlankRuleParameterDto()
        {
            return new RuleParameterDto
            {
                CancelledOrders = new CancelledOrderRuleParameterDto[0],
                HighProfits = new HighProfitsRuleParameterDto[0],
                MarkingTheCloses = new MarkingTheCloseRuleParameterDto[0],
                Spoofings = new SpoofingRuleParameterDto[0],
                Layerings = new LayeringRuleParameterDto[0],
                HighVolumes = new HighVolumeRuleParameterDto[0],
                WashTrades = new WashTradeRuleParameterDto[0],
                Rampings = new RampingRuleParameterDto[0],
                PlacingOrders = new PlacingOrdersWithNoIntentToExecuteRuleParameterDto[0],
                FixedIncomeHighVolumeIssuance = new FixedIncomeHighVolumeRuleParameterDto[0],
                FixedIncomeHighProfits = new FixedIncomeHighProfitRuleParameterDto[0],
                FixedIncomeWashTrades = new FixedIncomeWashTradeRuleParameterDto[0]
            };
        }
    }
}
