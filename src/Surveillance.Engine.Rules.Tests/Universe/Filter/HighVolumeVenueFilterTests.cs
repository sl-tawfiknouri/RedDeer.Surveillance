namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    using System;
    using System.Linq;

    using FakeItEasy;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using NUnit.Framework;

    using RedDeer.Contracts.SurveillanceService.Api.Markets;

    using SharedKernel.Contracts.Markets;

    using Surveillance.Auditing.Context.Interfaces;
    using Surveillance.Data.Universe.Interfaces;
    using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
    using Surveillance.Engine.Rules.Factories.Interfaces;
    using Surveillance.Engine.Rules.Markets;
    using Surveillance.Engine.Rules.Markets.Interfaces;
    using Surveillance.Engine.Rules.RuleParameters;
    using Surveillance.Engine.Rules.RuleParameters.Filter;
    using Surveillance.Engine.Rules.Rules;
    using Surveillance.Engine.Rules.Trades;
    using Surveillance.Engine.Rules.Universe.Filter;
    using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
    using Surveillance.Engine.Rules.Universe.Interfaces;
    using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

    [TestFixture]
    public class HighVolumeVenueFilterTests
    {
        private ILogger _baseLogger;

        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;

        private DecimalRangeRuleFilter _decimalRangeRuleFilter;

        private ILogger<HighVolumeVenueFilter> _logger;

        // plain fake
        private IMarketTradingHoursService _marketTradingHoursService;

        private ISystemProcessOperationRunRuleContext _ruleRunContext;

        private RuleRunMode _ruleRunMode;

        private TimeWindows _timeWindows;

        // populated with mocked ops
        private IMarketTradingHoursService _tradingHoursService;

        private ILogger<TradingHistoryStack> _tradingLogger;

        private IUniverseMarketCacheFactory _universeMarketCacheFactory;

        private IUniverseOrderFilter _universeOrderFilter;

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(
                () => new HighVolumeVenueFilter(
                    this._timeWindows,
                    this._decimalRangeRuleFilter,
                    this._universeOrderFilter,
                    this._ruleRunContext,
                    this._universeMarketCacheFactory,
                    this._ruleRunMode,
                    this._marketTradingHoursService,
                    this._dataRequestSubscriber,
                    DataSource.AllInterday,
                    this._tradingLogger,
                    null));
        }

        [SetUp]
        public void Setup()
        {
            this._universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            this._ruleRunContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            this._ruleRunMode = RuleRunMode.ValidationRun;
            this._universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            this._timeWindows = new TimeWindows("id-1", TimeSpan.FromDays(1));
            this._decimalRangeRuleFilter = new DecimalRangeRuleFilter();
            this._marketTradingHoursService = A.Fake<IMarketTradingHoursService>();
            this._dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            this._baseLogger = A.Fake<ILogger>();
            this._tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();
            this._logger = A.Fake<ILogger<HighVolumeVenueFilter>>();

            A.CallTo(() => this._universeOrderFilter.Filter(A<IUniverseEvent>.Ignored))
                .ReturnsLazily(_ => _.Arguments.First() as IUniverseEvent);

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A.CallTo(() => repository.GetAsync()).Returns(
                new[]
                    {
                        new ExchangeDto
                            {
                                Code = "XLON",
                                MarketOpenTime = TimeSpan.FromHours(8),
                                MarketCloseTime = TimeSpan.FromHours(16),
                                IsOpenOnMonday = true,
                                IsOpenOnTuesday = true,
                                IsOpenOnWednesday = true,
                                IsOpenOnThursday = true,
                                IsOpenOnFriday = true,
                                IsOpenOnSaturday = true,
                                IsOpenOnSunday = true
                            },
                        new ExchangeDto
                            {
                                Code = "NASDAQ",
                                MarketOpenTime = TimeSpan.FromHours(15),
                                MarketCloseTime = TimeSpan.FromHours(23),
                                IsOpenOnMonday = true,
                                IsOpenOnTuesday = true,
                                IsOpenOnWednesday = true,
                                IsOpenOnThursday = true,
                                IsOpenOnFriday = true,
                                IsOpenOnSaturday = true,
                                IsOpenOnSunday = true
                            }
                    });

            this._tradingHoursService = new MarketTradingHoursService(
                repository,
                new NullLogger<MarketTradingHoursService>());
        }
    }
}