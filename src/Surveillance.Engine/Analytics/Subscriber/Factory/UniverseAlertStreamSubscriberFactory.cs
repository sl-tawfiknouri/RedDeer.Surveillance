using System;
using Microsoft.Extensions.Logging;
using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory
{
    public class UniverseAlertStreamSubscriberFactory : IUniverseAlertStreamSubscriberFactory
    {
        private readonly ICancelledOrderRuleCachedMessageSender _cancelledOrderMessageSender;
        private readonly IHighProfitRuleCachedMessageSender _highProfitMessageSender;
        private readonly IHighVolumeRuleCachedMessageSender _highVolumeMessageSender;
        private readonly ILayeringCachedMessageSender _layeringCachedMessageSender;
        private readonly IMarkingTheCloseMessageSender _markingTheCloseMessageSender;
        private readonly ISpoofingRuleMessageSender _spoofingMessageSender;
        private readonly IWashTradeCachedMessageSender _washTradeMessageSender;
        private readonly ILogger<IUniverseAlertSubscriber> _logger;

        public UniverseAlertStreamSubscriberFactory(
            ICancelledOrderRuleCachedMessageSender cancelledOrderMessageSender,
            IHighProfitRuleCachedMessageSender highProfitMessageSender,
            IHighVolumeRuleCachedMessageSender highVolumeMessageSender,
            ILayeringCachedMessageSender layeringMessageSender,
            IMarkingTheCloseMessageSender markingTheCloseMessageSender,
            ISpoofingRuleMessageSender spoofingMessageSender,
            IWashTradeCachedMessageSender washTradeMessageSender,
            ILogger<IUniverseAlertSubscriber> logger)
        {
            _cancelledOrderMessageSender =
                cancelledOrderMessageSender
                ?? throw new ArgumentNullException(nameof(cancelledOrderMessageSender));

            _highProfitMessageSender =
                highProfitMessageSender
                ?? throw new ArgumentNullException(nameof(highProfitMessageSender));

            _highVolumeMessageSender =
                highVolumeMessageSender
                ?? throw new ArgumentNullException(nameof(highVolumeMessageSender));

            _layeringCachedMessageSender =
                layeringMessageSender
                ?? throw new ArgumentNullException(nameof(layeringMessageSender));

            _markingTheCloseMessageSender =
                markingTheCloseMessageSender
                ?? throw new ArgumentNullException(nameof(markingTheCloseMessageSender));

            _spoofingMessageSender =
                spoofingMessageSender
                ?? throw new ArgumentNullException(nameof(spoofingMessageSender));

            _washTradeMessageSender =
                washTradeMessageSender
                ?? throw new ArgumentNullException(nameof(washTradeMessageSender));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseAlertSubscriber Build(int opCtxId, bool isBackTest)
        {
            return new UniverseAlertsSubscriber(
                opCtxId,
                isBackTest,
                _cancelledOrderMessageSender,
                _highProfitMessageSender,
                _highVolumeMessageSender,
                _layeringCachedMessageSender,
                _markingTheCloseMessageSender,
                _spoofingMessageSender,
                _washTradeMessageSender,
                _logger);
        }
    }
}
