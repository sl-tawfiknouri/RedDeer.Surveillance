using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Analytics.Subscriber.Interfaces;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;
using Surveillance.Trades.Interfaces;

namespace Surveillance.Analytics.Subscriber
{
    /// <summary>
    /// This is primarily for cross-rule analytics
    /// In the future it will be used for calculating correlations between underlying positions
    /// And perhaps aggregating alerts from multiple rules
    /// We share one stream per scheduled execution so we can perform cross analysis on alerts raised
    /// </summary>
    public class UniverseAlertsSubscriber : IUniverseAlertSubscriber
    {
        private readonly ICancelledOrderRuleCachedMessageSender _cancelledOrderMessageSender;
        private readonly IHighProfitRuleCachedMessageSender _highProfitMessageSender;
        private readonly IHighVolumeRuleCachedMessageSender _highVolumeMessageSender;
        private readonly ILayeringCachedMessageSender _layeringCachedMessageSender;
        private readonly IMarkingTheCloseMessageSender _markingTheCloseMessageSender;
        private readonly ISpoofingRuleMessageSender _spoofingMessageSender;
        private readonly IWashTradeCachedMessageSender _washTradeMessageSender;
        private readonly ILogger<IUniverseAlertSubscriber> _logger;

        public UniverseAlertsSubscriber(
            int opCtxId,
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
            Analytics = new AlertAnalytics {SystemProcessOperationId = opCtxId};
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        { }

        public void OnNext(IUniverseAlertEvent value)
        {
            switch (value.Rule)
            {
                case Domain.Scheduling.Rules.CancelledOrders:
                    CancelledOrders(value);
                    break;
                case Domain.Scheduling.Rules.HighProfits:
                    HighProfits(value);
                    break;
                case Domain.Scheduling.Rules.HighVolume:
                    HighVolume(value);
                    break;
                case Domain.Scheduling.Rules.Layering:
                    Layering(value);
                    break;
                case Domain.Scheduling.Rules.MarkingTheClose:
                    MarkingTheClose(value);
                    break;
                case Domain.Scheduling.Rules.Spoofing:
                    Spoofing(value);
                    break;
                case Domain.Scheduling.Rules.WashTrade:
                    WashTrade(value);
                    break;
                default:
                    _logger.LogError($"UniverseAlertsSubscriber met a rule type it did not identify {value.Rule}. This should be explicitly addressed.");
                    break;
            }
        }

        private void CancelledOrders(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                Analytics.CancelledOrderAlertsAdjusted = _cancelledOrderMessageSender.Flush(alert.Context);
                return;
            }

            var ruleBreach = (ICancelledOrderRuleBreach)alert.UnderlyingAlert;
            _cancelledOrderMessageSender.Send(ruleBreach);

            Analytics.CancelledOrderAlertsRaw += 1;
        }

        private void HighProfits(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                Analytics.HighProfitAlertsAdjusted = _highProfitMessageSender.Flush(alert.Context);
                return;
            }

            if (alert.IsRemoveEvent)
            {
                _highProfitMessageSender.Remove((ITradePosition)alert.UnderlyingAlert);
                return;
            }

            var ruleBreach = (IHighProfitRuleBreach) alert.UnderlyingAlert;
            _highProfitMessageSender.Send(ruleBreach);

            Analytics.HighProfitAlertsRaw += 1;
        }

        private void HighVolume(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                Analytics.HighVolumeAlertsAdjusted = _highVolumeMessageSender.Flush(alert.Context);
                return;
            }

            var ruleBreach = (IHighVolumeRuleBreach)alert.UnderlyingAlert;
            _highVolumeMessageSender.Send(ruleBreach);

            Analytics.HighVolumeAlertsRaw += 1;
        }

        private void Layering(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                Analytics.LayeringAlertsAdjusted = _layeringCachedMessageSender.Flush(alert.Context);
                return;
            }

            var ruleBreach = (ILayeringRuleBreach)alert.UnderlyingAlert;
            _layeringCachedMessageSender.Send(ruleBreach);

            Analytics.LayeringAlertsRaw += 1;
        }

        private void MarkingTheClose(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (IMarkingTheCloseBreach)alert.UnderlyingAlert;
            _markingTheCloseMessageSender.Send(ruleBreach, alert.Context);

            Analytics.MarkingTheCloseAlertsRaw += 1;
            Analytics.MarkingTheCloseAlertsAdjusted += 1;
        }

        private void Spoofing(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (ISpoofingRuleBreach)alert.UnderlyingAlert;
            _spoofingMessageSender.Send(ruleBreach, alert.Context);

            Analytics.SpoofingAlertsRaw += 1;
            Analytics.SpoofingAlertsAdjusted += 1;
        }

        private void WashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                Analytics.WashTradeAlertsAdjusted = _washTradeMessageSender.Flush(alert.Context);
                return;
            }

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            _washTradeMessageSender.Send(ruleBreach);

            Analytics.WashTradeAlertsRaw += 1;
        }

        public AlertAnalytics Analytics { get; }
    }
}