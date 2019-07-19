using System;
using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using RedDeer.Contracts.SurveillanceService.Api.Markets;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.DataLayer.Aurora.BMLL;
using Surveillance.DataLayer.Aurora.Interfaces;
using Surveillance.Engine.Rules.Data.Subscribers.Interfaces;
using Surveillance.Engine.Rules.Factories;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;
using Surveillance.Engine.Rules.Universe.Interfaces;
using Surveillance.Reddeer.ApiClient.MarketOpenClose.Interfaces;

namespace Surveillance.Engine.Rules.Tests.Universe.Filter
{
    [TestFixture]
    public class HighVolumeVenueFilterTests
    {
        private IUniverseMarketCacheFactory _universeMarketCacheFactory;
        private ISystemProcessOperationRunRuleContext _ruleRunContext;
        private RuleRunMode _ruleRunMode;
        private IUniverseOrderFilter _universeOrderFilter;
        private TimeWindows _timeWindows;
        private DecimalRangeRuleFilter _decimalRangeRuleFilter;
        // plain fake
        private IMarketTradingHoursService _marketTradingHoursService;
        // populated with mocked ops
        private IMarketTradingHoursService _tradingHoursService;
        private IUniverseDataRequestsSubscriber _dataRequestSubscriber;
        private ILogger _baseLogger;
        private ILogger<TradingHistoryStack> _tradingLogger;
        private ILogger<HighVolumeVenueFilter> _logger;

        [SetUp]
        public void Setup()
        {
            _universeMarketCacheFactory = A.Fake<IUniverseMarketCacheFactory>();
            _ruleRunContext = A.Fake<ISystemProcessOperationRunRuleContext>();
            _ruleRunMode = RuleRunMode.ValidationRun;
            _universeOrderFilter = A.Fake<IUniverseOrderFilter>();
            _timeWindows = new TimeWindows("id-1", TimeSpan.FromDays(1));
            _decimalRangeRuleFilter = new DecimalRangeRuleFilter();
            _marketTradingHoursService = A.Fake<IMarketTradingHoursService>();
            _dataRequestSubscriber = A.Fake<IUniverseDataRequestsSubscriber>();
            _baseLogger = A.Fake<ILogger>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();
            _logger = A.Fake<ILogger<HighVolumeVenueFilter>>();

            A
                .CallTo(() => _universeOrderFilter.Filter(A<IUniverseEvent>.Ignored))
                .ReturnsLazily(_ => _.Arguments.First() as IUniverseEvent);

            var repository = A.Fake<IMarketOpenCloseApiCachingDecorator>();

            A
                .CallTo(() => repository.Get()).
                Returns(new ExchangeDto[]
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
                        IsOpenOnSunday = true,
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
                        IsOpenOnSunday = true,
                    }
                });

            _tradingHoursService = new MarketTradingHoursService(repository, new NullLogger<MarketTradingHoursService>());
        }

        [Test]
        public void Ctor_NullLogger_IsExceptional()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Assert.Throws<ArgumentNullException>(() =>
                new HighVolumeVenueFilter(
                    _timeWindows,
                    _decimalRangeRuleFilter,
                    _universeOrderFilter,
                    _ruleRunContext,
                    _universeMarketCacheFactory,
                    _ruleRunMode,
                    _marketTradingHoursService,
                    _dataRequestSubscriber,
                    _tradingLogger,
                    null));
        }
    }
}
