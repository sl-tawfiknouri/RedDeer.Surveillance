using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Factories.Interfaces;
using Surveillance.Markets.Interfaces;
using Surveillance.MessageBusIO.Interfaces;
using Surveillance.RuleParameters.Interfaces;
using Surveillance.Rules;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;
using Surveillance.Trades;
using Surveillance.Universe.Filter.Interfaces;

namespace Surveillance.Factories
{
    public class HighVolumeRuleFactory : IHighVolumeRuleFactory
    {
        private readonly IUniverseOrderFilter _orderFilter;
        private readonly IUniverseMarketCacheFactory _factory;
        private readonly IMarketTradingHoursManager _tradingHoursManager;
        private readonly IDataRequestMessageSender _messageSender;
        private readonly ILogger<IHighVolumeRule> _logger;
        private readonly ILogger<TradingHistoryStack> _tradingHistoryLogger;

        public HighVolumeRuleFactory(
            IUniverseOrderFilter orderFilter,
            IUniverseMarketCacheFactory factory,
            IMarketTradingHoursManager tradingHoursManager,
            IDataRequestMessageSender messageSender,
            ILogger<IHighVolumeRule> logger,
            ILogger<TradingHistoryStack> tradingHistoryLogger)
        {
            _orderFilter = orderFilter ?? throw new ArgumentNullException(nameof(IUniverseOrderFilter));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _tradingHoursManager = tradingHoursManager ?? throw new ArgumentNullException(nameof(tradingHoursManager));
            _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tradingHistoryLogger = tradingHistoryLogger ?? throw new ArgumentNullException(nameof(tradingHistoryLogger));
        }

        public IHighVolumeRule Build(
            IHighVolumeRuleParameters parameters,
            ISystemProcessOperationRunRuleContext opCtx,
            IUniverseAlertStream alertStream)
        {

            return new HighVolumeRule(parameters, opCtx, alertStream, _orderFilter, _factory, _tradingHoursManager, _messageSender, _logger, _tradingHistoryLogger);
        }

        public static string Version => Versioner.Version(1, 0);
    }
}
