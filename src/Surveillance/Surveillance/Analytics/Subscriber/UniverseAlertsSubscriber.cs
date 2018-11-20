using System;
using Microsoft.Extensions.Logging;
using Surveillance.Analytics.Streams.Interfaces;
using Surveillance.Analytics.Subscriber.Interfaces;
using Surveillance.Rules.CancelledOrders.Interfaces;
using Surveillance.Rules.HighProfits.Interfaces;
using Surveillance.Rules.HighVolume;
using Surveillance.Rules.HighVolume.Interfaces;
using Surveillance.Rules.Layering.Interfaces;
using Surveillance.Rules.MarkingTheClose.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Rules.WashTrade.Interfaces;

namespace Surveillance.Analytics.Subscriber
{
    /// <summary>
    /// This is primarily for cross-rule analytics
    /// In the future it will be used for calculating correlations between underlying positions
    /// And perhaps aggregating alerts from multiple rules
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
                _cancelledOrderMessageSender.Flush(alert.Context);

                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (ICancelledOrderRuleBreach)alert.UnderlyingAlert;
            _cancelledOrderMessageSender.Send(ruleBreach);
        }

        private void HighProfits(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                _highProfitMessageSender.Flush(alert.Context);

                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (IHighProfitRuleBreach) alert.UnderlyingAlert;
            _highProfitMessageSender.Send(ruleBreach);
        }

        private void HighVolume(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                _highVolumeMessageSender.Flush(alert.Context);

                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (IHighVolumeRuleBreach)alert.UnderlyingAlert;
            _highVolumeMessageSender.Send(ruleBreach);
        }

        private void Layering(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                _layeringCachedMessageSender.Flush(alert.Context);

                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (ILayeringRuleBreach)alert.UnderlyingAlert;
            _layeringCachedMessageSender.Send(ruleBreach);
        }

        private void MarkingTheClose(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (IMarkingTheCloseBreach)alert.UnderlyingAlert;
            _markingTheCloseMessageSender.Send(ruleBreach, alert.Context);
        }

        private void Spoofing(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (ISpoofingRuleBreach)alert.UnderlyingAlert;
            _spoofingMessageSender.Send(ruleBreach, alert.Context);
        }

        private void WashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                _washTradeMessageSender.Flush(alert.Context);

                // TODO think more about this line
                alert.Context?.UpdateAlertEvent(0);
                return;
            }

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            _washTradeMessageSender.Send(ruleBreach);
        }
    }
}