using System;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Surveillance.Auditing.Context.Interfaces;
using Surveillance.Engine.Rules.Factories.Interfaces;
using Surveillance.Engine.Rules.Markets.Interfaces;
using Surveillance.Engine.Rules.RuleParameters;
using Surveillance.Engine.Rules.RuleParameters.Filter;
using Surveillance.Engine.Rules.Rules;
using Surveillance.Engine.Rules.Trades;
using Surveillance.Engine.Rules.Universe.Filter;
using Surveillance.Engine.Rules.Universe.Filter.Interfaces;

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
        private IMarketTradingHoursService _marketTradingHoursService;
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
            _baseLogger = A.Fake<ILogger>();
            _tradingLogger = A.Fake<ILogger<TradingHistoryStack>>();
            _logger = A.Fake<ILogger<HighVolumeVenueFilter>>();
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
                    _baseLogger,
                    _tradingLogger,
                    null));
        }
    }
}
