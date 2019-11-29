using Dapper;
using DataImport;
using DataImport.Configuration;
using DataImport.Configuration.Interfaces;
using DataImport.Disk_IO.AllocationFile.Interfaces;
using DataImport.Disk_IO.Interfaces;
using DataSynchroniser;
using DataSynchroniser.Api.Bmll;
using DataSynchroniser.Api.Factset;
using DataSynchroniser.Api.Markit;
using DataSynchroniser.Configuration;
using DataSynchroniser.Queues.Interfaces;
using Domain.Core.Trading.Orders;
using FakeItEasy;
using FluentAssertions;
using Infrastructure.Network.Aws.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using NLog.Web;
using RedDeer.Contracts.SurveillanceService.Api.FactsetSecurityDaily;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.Equities;
using RedDeer.Contracts.SurveillanceService.Api.RuleParameter.FixedIncome;
using RedDeer.Contracts.SurveillanceService.Rules;
using RedDeer.Surveillance.App;
using RedDeer.Surveillance.App.ScriptRunner.Interfaces;
using SharedKernel.Contracts.Queues;
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
using Surveillance.Reddeer.ApiClient.FactsetMarketData.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;
using Surveillance.Reddeer.ApiClient.RuleParameter.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RedDeer.Surveillance.IntegrationTests.Runner
{
    public class RuleRunner
    {
        public static readonly string DatabaseConfig = @"server=127.0.0.1; port=3306;uid=root;pwd='drunkrabbit101';database=test_surveillance; Allow User Variables=True";

        public static readonly string CorrelationId = "testCorrelation";
        public static readonly string RuleId = "testRule";
        public static readonly string MessageId = "testMessage";

        public RuleParameterDto RuleParameterDto = new RuleParameterDto
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

        public string TradeCsvContent { get; set; }
        public string AllocationCsvContent { get; set; }

        public int ExpectedOrderCount { get; set; } = 0;
        public int ExpectedAllocationCount { get; set; } = 0;

        public EquityClosePriceMock EquityClosePriceMock = new EquityClosePriceMock();

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

                await RunRule(false);

                var dataRequestOperationId = GetDataRequestOperationId(dbContext);
                if (dataRequestOperationId.HasValue)
                {
                    await RunDataSynchroniser(dataRequestOperationId.Value);
                    await RunRule(true);
                }

                OriginalRuleBreaches = GetRuleBreaches(dbContext);
                RemainingRuleBreaches = OriginalRuleBreaches.ToList();

                PrintBreaches();
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
            Console.WriteLine("Setting up database");

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
                truncate ruleanalyticsalerts;
                truncate ruleanalyticsuniverse;
                delete from systemprocessoperationrulerun where id > 0;
                delete from systemprocessoperationdatasynchroniserrequest where id > 0;
                delete from systemprocessoperation where id > 0;
                delete from systemprocess where id is not null;
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
            Console.WriteLine("Running data import");

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

        private async Task RunRule(bool isForceRun)
        {
            Console.WriteLine($"Running rule engine (isForceRun = {isForceRun})");

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
            A.CallTo(() => ruleParameterApi.GetAsync(RuleId)).Returns(Task.FromResult(RuleParameterDto));
            container.Inject(typeof(IRuleParameterApi), ruleParameterApi);

            // replace market open close api
            var marketOpenCloseApi = A.Fake<IMarketOpenCloseApi>();

            var exchanges = JsonConvert.DeserializeObject<List<ExchangeDto>>(@"[
                {
		            ""Code"": ""XLON"",
		            ""CountryCode"": ""GB"",
		            ""Name"": ""London Stock Exchange"",
		            ""AcquireCode"": ""XC/LSE"",
		            ""ReutersCode"": "".L"",
		            ""BloombergCode"": ""LN"",
		            ""StocktwitsCode"": """",
		            ""MarketOpenTime"": ""08:00:00"",
		            ""MarketCloseTime"": ""16:35:00"",
		            ""BeforeMarketTime"": ""07:00:00"",
		            ""AfterMarketTime"": ""16:30:00"",
		            ""TimeZone"": ""GMT Standard Time"",
		            ""Holidays"": [""1995-01-02T00:00:00"", ""1995-04-14T00:00:00"", ""1995-04-17T00:00:00"", ""1995-05-08T00:00:00"", ""1995-05-29T00:00:00"", ""1995-08-28T00:00:00"", ""1995-12-25T00:00:00"", ""1995-12-26T00:00:00"", ""1996-01-01T00:00:00"", ""1996-04-05T00:00:00"", ""1996-04-08T00:00:00"", ""1996-05-06T00:00:00"", ""1996-05-27T00:00:00"", ""1996-08-26T00:00:00"", ""1996-12-25T00:00:00"", ""1996-12-26T00:00:00"", ""1997-01-01T00:00:00"", ""1997-03-28T00:00:00"", ""1997-03-31T00:00:00"", ""1997-05-05T00:00:00"", ""1997-05-26T00:00:00"", ""1997-08-25T00:00:00"", ""1997-12-25T00:00:00"", ""1997-12-26T00:00:00"", ""1998-01-01T00:00:00"", ""1998-04-10T00:00:00"", ""1998-04-13T00:00:00"", ""1998-05-04T00:00:00"", ""1998-05-25T00:00:00"", ""1998-08-31T00:00:00"", ""1998-12-25T00:00:00"", ""1998-12-28T00:00:00"", ""1998-12-31T00:00:00"", ""1999-01-01T00:00:00"", ""1999-04-02T00:00:00"", ""1999-04-05T00:00:00"", ""1999-05-03T00:00:00"", ""1999-05-31T00:00:00"", ""1999-08-30T00:00:00"", ""1999-12-27T00:00:00"", ""1999-12-28T00:00:00"", ""1999-12-31T00:00:00"", ""2000-01-03T00:00:00"", ""2000-04-21T00:00:00"", ""2000-04-24T00:00:00"", ""2000-05-01T00:00:00"", ""2000-05-29T00:00:00"", ""2000-08-28T00:00:00"", ""2000-12-25T00:00:00"", ""2000-12-26T00:00:00"", ""2001-01-01T00:00:00"", ""2001-04-13T00:00:00"", ""2001-04-16T00:00:00"", ""2001-05-07T00:00:00"", ""2001-05-28T00:00:00"", ""2001-08-27T00:00:00"", ""2001-12-25T00:00:00"", ""2001-12-26T00:00:00"", ""2002-01-01T00:00:00"", ""2002-03-29T00:00:00"", ""2002-04-01T00:00:00"", ""2002-05-06T00:00:00"", ""2002-06-03T00:00:00"", ""2002-06-04T00:00:00"", ""2002-08-26T00:00:00"", ""2002-12-25T00:00:00"", ""2002-12-26T00:00:00"", ""2003-01-01T00:00:00"", ""2003-04-18T00:00:00"", ""2003-04-21T00:00:00"", ""2003-05-05T00:00:00"", ""2003-05-26T00:00:00"", ""2003-08-25T00:00:00"", ""2003-12-25T00:00:00"", ""2003-12-26T00:00:00"", ""2004-01-01T00:00:00"", ""2004-04-09T00:00:00"", ""2004-04-12T00:00:00"", ""2004-05-03T00:00:00"", ""2004-05-31T00:00:00"", ""2004-08-30T00:00:00"", ""2004-12-27T00:00:00"", ""2004-12-28T00:00:00"", ""2005-01-03T00:00:00"", ""2005-03-25T00:00:00"", ""2005-03-28T00:00:00"", ""2005-05-02T00:00:00"", ""2005-05-30T00:00:00"", ""2005-08-29T00:00:00"", ""2005-12-26T00:00:00"", ""2005-12-27T00:00:00"", ""2006-01-02T00:00:00"", ""2006-04-14T00:00:00"", ""2006-04-17T00:00:00"", ""2006-05-01T00:00:00"", ""2006-05-29T00:00:00"", ""2006-08-28T00:00:00"", ""2006-12-25T00:00:00"", ""2006-12-26T00:00:00"", ""2007-01-01T00:00:00"", ""2007-04-06T00:00:00"", ""2007-04-09T00:00:00"", ""2007-05-07T00:00:00"", ""2007-05-28T00:00:00"", ""2007-08-27T00:00:00"", ""2007-12-25T00:00:00"", ""2007-12-26T00:00:00"", ""2008-01-01T00:00:00"", ""2008-03-21T00:00:00"", ""2008-03-24T00:00:00"", ""2008-05-05T00:00:00"", ""2008-05-26T00:00:00"", ""2008-08-25T00:00:00"", ""2008-12-25T00:00:00"", ""2008-12-26T00:00:00"", ""2009-01-01T00:00:00"", ""2009-04-10T00:00:00"", ""2009-04-13T00:00:00"", ""2009-05-04T00:00:00"", ""2009-05-25T00:00:00"", ""2009-08-31T00:00:00"", ""2009-12-25T00:00:00"", ""2009-12-28T00:00:00"", ""2010-01-01T00:00:00"", ""2010-04-02T00:00:00"", ""2010-04-05T00:00:00"", ""2010-05-03T00:00:00"", ""2010-05-31T00:00:00"", ""2010-08-30T00:00:00"", ""2010-12-27T00:00:00"", ""2010-12-28T00:00:00"", ""2011-01-03T00:00:00"", ""2011-04-22T00:00:00"", ""2011-04-25T00:00:00"", ""2011-04-29T00:00:00"", ""2011-05-02T00:00:00"", ""2011-05-30T00:00:00"", ""2011-08-29T00:00:00"", ""2011-12-26T00:00:00"", ""2011-12-27T00:00:00"", ""2012-01-02T00:00:00"", ""2012-04-06T00:00:00"", ""2012-04-09T00:00:00"", ""2012-05-07T00:00:00"", ""2012-06-04T00:00:00"", ""2012-06-05T00:00:00"", ""2012-08-27T00:00:00"", ""2012-12-25T00:00:00"", ""2012-12-26T00:00:00"", ""2013-01-01T00:00:00"", ""2013-03-29T00:00:00"", ""2013-04-01T00:00:00"", ""2013-05-06T00:00:00"", ""2013-05-27T00:00:00"", ""2013-08-26T00:00:00"", ""2013-12-25T00:00:00"", ""2013-12-26T00:00:00"", ""2014-01-01T00:00:00"", ""2014-04-18T00:00:00"", ""2014-04-21T00:00:00"", ""2014-05-05T00:00:00"", ""2014-05-26T00:00:00"", ""2014-08-25T00:00:00"", ""2014-12-25T00:00:00"", ""2014-12-26T00:00:00"", ""2015-01-01T00:00:00"", ""2015-04-03T00:00:00"", ""2015-04-06T00:00:00"", ""2015-05-04T00:00:00"", ""2015-05-25T00:00:00"", ""2015-08-31T00:00:00"", ""2015-12-25T00:00:00"", ""2015-12-28T00:00:00"", ""2016-01-01T00:00:00"", ""2016-03-25T00:00:00"", ""2016-03-28T00:00:00"", ""2016-05-02T00:00:00"", ""2016-05-30T00:00:00"", ""2016-08-29T00:00:00"", ""2016-12-26T00:00:00"", ""2016-12-27T00:00:00""],
		            ""LoadSecuritiesFromSecurityMaster"": false,
		            ""PriceDelayedFeed"": false,
		            ""PriceRealTimeFeed"": false,
		            ""UseCompositeBbgId"": false,
		            ""IsOpenOnMonday"": true,
		            ""IsOpenOnTuesday"": true,
		            ""IsOpenOnWednesday"": true,
		            ""IsOpenOnThursday"": true,
		            ""IsOpenOnFriday"": true,
		            ""IsOpenOnSaturday"": false,
		            ""IsOpenOnSunday"": false,
		            ""BenchmarkSecurityId"": ""RD001039440"",
		            ""TotalReturnBenchmarkSecurityId"": ""RD001039439"",
		            ""DisableVolumeCheckForPriceHistory"": false
	            }
            ]");
            A.CallTo(() => marketOpenCloseApi.GetAsync()).Returns(exchanges);

            container.Inject(typeof(IMarketOpenCloseApi), marketOpenCloseApi);

            // execution message
            var execution = new ScheduledExecution
            {
                TimeSeriesInitiation = new DateTimeOffset(From),
                TimeSeriesTermination = new DateTimeOffset(To),
                CorrelationId = CorrelationId,
                IsBackTest = true,
                IsForceRerun = isForceRun,
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
            if (RuleParameterDto.WashTrades.Length > 0)
            {
                return Rules.WashTrade;
            }
            if (RuleParameterDto.HighProfits.Length > 0)
            {
                return Rules.HighProfits;
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

        private int? GetDataRequestOperationId(IGraphQlDbContext dbContext)
        {
            var ruleRunIds = dbContext
                .RuleDataRequest
                .Select(x => x.SystemProcessOperationRuleRunId)
                .Distinct()
                .ToList();

            if (ruleRunIds.Count == 0)
            {
                return null;
            }

            if (ruleRunIds.Count > 1)
            {
                throw new Exception("RuleDataRequest table contains more than one distinct SystemProcessOperationRuleRunId");
            }

            var operationIds = dbContext
                .RuleRun
                .Where(x => x.Id == ruleRunIds.First())
                .Select(x => x.SystemProcessOperationId)
                .Distinct()
                .ToList();

            return operationIds.Single();
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

        private void PrintBreaches()
        {
            var breaches = OriginalRuleBreaches
                .Select(x => new
                {
                    Id = x.RuleBreach.Id,
                    Description = x.RuleBreach.Description,
                    Orders = x.Orders.Select(y => new
                    {
                        ClientOrderId = y.ClientOrderId,
                        Direction = ((OrderDirections)y.Direction).ToString(),
                        AverageFillPrice = y.AverageFillPrice
                    })
                });

            var json = JsonConvert.SerializeObject(breaches); 
            var jsonFormatted = JValue.Parse(json).ToString(Formatting.Indented);

            Console.WriteLine("");
            Console.WriteLine("Breaches:");
            Console.WriteLine(jsonFormatted);
            Console.WriteLine("");
        }

        private async Task RunDataSynchroniser(int operationId)
        {
            Console.WriteLine("Running data synchroniser");

            var container = new Container();

            // registries
            container.Configure(
                    config =>
                    {
                        config.IncludeRegistry<DataSynchroniserRegistry>();
                        config.IncludeRegistry<SurveillanceSystemAuditingRegistry>();
                        config.IncludeRegistry<SystemSystemDataLayerRegistry>();
                        config.IncludeRegistry<BmllDataSynchroniserRegistry>();
                        config.IncludeRegistry<ReddeerApiClientRegistry>();
                        config.IncludeRegistry<DataLayerRegistry>();
                        config.IncludeRegistry<FactsetDataSynchroniserRegistry>();
                        config.IncludeRegistry<MarkitDataSynchroniserRegistry>();
                    });

            // data synchroniser config
            var dataSynchroniserConfig = new Config
            {
                AuroraConnectionString = DatabaseConfig,
                SurveillanceAuroraConnectionString = DatabaseConfig
            };
            container.Inject(typeof(IAwsConfiguration), dataSynchroniserConfig);
            container.Inject(typeof(ISystemDataLayerConfig), dataSynchroniserConfig);
            container.Inject(typeof(IApiClientConfiguration), dataSynchroniserConfig);
            container.Inject(typeof(IDataLayerConfiguration), dataSynchroniserConfig);

            // replace aws queue client with fake
            var awsQueueClient = A.Fake<IAwsQueueClient>();
            container.Inject(typeof(IAwsQueueClient), awsQueueClient);

            // replace factset daily bar api
            var factsetDailyBarApi = A.Fake<IFactsetDailyBarApi>();
            A.CallTo(() => factsetDailyBarApi.GetWithTransientFaultHandlingAsync(A<FactsetSecurityDailyRequest>._))
                .ReturnsLazily(input => Task.FromResult(EquityClosePriceMock.GetPrices((FactsetSecurityDailyRequest) input.Arguments.First())));
            container.Inject(typeof(IFactsetDailyBarApi), factsetDailyBarApi);

            // data request message
            var dataRequestMessage = new ThirdPartyDataRequestMessage
            {
                SystemProcessOperationId = operationId.ToString()
            };
            
            // run data synchroniser
            var dataRequestSubscriber = container.GetInstance<IDataRequestSubscriber>();
            await dataRequestSubscriber.Execute(MessageId, JsonConvert.SerializeObject(dataRequestMessage));
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
