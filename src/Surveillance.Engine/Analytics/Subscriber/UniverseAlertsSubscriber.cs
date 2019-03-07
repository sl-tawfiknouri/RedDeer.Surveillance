using System;
using Microsoft.Extensions.Logging;
using Surveillance.DataLayer.Aurora.Analytics;
using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighProfits.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
using Surveillance.Engine.Rules.Rules.Equity.WashTrade.Interfaces;
using Surveillance.Engine.Rules.Rules.Interfaces;
using Surveillance.Engine.Rules.Trades.Interfaces;

namespace Surveillance.Engine.Rules.Analytics.Subscriber
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
        private readonly IWashTradeCachedMessageSender _equityWashTradeMessageSender;
        private readonly IWashTradeCachedMessageSender _fixedIncomeWashTradeMessageSender;
        private readonly ILogger<IUniverseAlertSubscriber> _logger;

        private readonly bool _isBackTest;
        private bool _hasHighProfitDeleteRequest;

        public UniverseAlertsSubscriber(
            int opCtxId,
            bool isBackTest,
            ICancelledOrderRuleCachedMessageSender cancelledOrderMessageSender,
            IHighProfitRuleCachedMessageSender highProfitMessageSender,
            IHighVolumeRuleCachedMessageSender highVolumeMessageSender,
            ILayeringCachedMessageSender layeringMessageSender,
            IMarkingTheCloseMessageSender markingTheCloseMessageSender,
            ISpoofingRuleMessageSender spoofingMessageSender,
            IWashTradeCachedMessageSender equityWashTradeMessageSender,
            IWashTradeCachedMessageSender fixedIncomeWashTradeMessageSender,
            ILogger<IUniverseAlertSubscriber> logger)
        {
            _isBackTest = isBackTest;

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

            _equityWashTradeMessageSender =
                equityWashTradeMessageSender
                ?? throw new ArgumentNullException(nameof(equityWashTradeMessageSender));

            _fixedIncomeWashTradeMessageSender =
                fixedIncomeWashTradeMessageSender
                ?? throw new ArgumentNullException(nameof(fixedIncomeWashTradeMessageSender));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Analytics = new AlertAnalytics {SystemProcessOperationId = opCtxId};
        }

        public void OnCompleted()
        {
            _logger.LogInformation($"received the OnCompleted() event from the stream");
        }

        public void OnError(Exception error)
        {
            _logger.LogError($"received the OnError() event from the stream {error.Message} {error.InnerException?.Message}");
        }

        public void OnNext(IUniverseAlertEvent value)
        {
            _logger.LogInformation($"received on the OnNext() event from the alerts stream. Rule {value.Rule} operation context {value.Context?.Id()}");

            switch (value.Rule)
            {
                case Domain.Surveillance.Scheduling.Rules.CancelledOrders:
                    CancelledOrders(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.HighProfits:
                    HighProfits(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.HighVolume:
                    HighVolume(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.Layering:
                    Layering(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.MarkingTheClose:
                    MarkingTheClose(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.Spoofing:
                    Spoofing(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.WashTrade:
                    EquityWashTrade(value);
                    break;
                case Domain.Surveillance.Scheduling.Rules.FixedIncomeWashTrades:
                    FixedIncomeWashTrade(value);
                    break;
                default:
                    _logger.LogError($"met a rule type it did not identify {value.Rule}. This should be explicitly addressed.");
                    break;
            }
        }

        private void CancelledOrders(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (ICancelledOrderRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"cancelled orders adding alert to cancelled order message sender");
            _cancelledOrderMessageSender.Send(ruleBreach);

            _logger.LogInformation($"cancelled orders incrementing raw alert count by 1");
            Analytics.CancelledOrderAlertsRaw += 1;
        }

        private void CancelledOrdersFlush()
        {
            _logger.LogInformation($"cancelled orders flushing alerts");
            Analytics.CancelledOrderAlertsAdjusted += _cancelledOrderMessageSender.Flush();
        }

        private void HighProfits(IUniverseAlertEvent alert)
        {
            _hasHighProfitDeleteRequest = alert.IsDeleteEvent || _hasHighProfitDeleteRequest;

            if (_hasHighProfitDeleteRequest)
            {
                _highProfitMessageSender.Delete();
                return;
            }

            if (alert.IsFlushEvent)
            {
                return;
            }

            if (alert.IsRemoveEvent)
            {
                _logger.LogInformation($"high profits noted alert is duplicated and removing it");
                _highProfitMessageSender.Remove((ITradePosition)alert.UnderlyingAlert);
                return;
            }

            var ruleBreach = (IHighProfitRuleBreach) alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"high profits adding alert to high profits message sender");
            _highProfitMessageSender.Send(ruleBreach);

            _logger.LogInformation($"high profits incrementing raw alert count by 1");
            Analytics.HighProfitAlertsRaw += 1;
        }

        private void HighProfitsFlush()
        {
            if (_hasHighProfitDeleteRequest)
            {
                return;
            }

            _logger.LogInformation($"high profits flushing alerts");
            Analytics.HighProfitAlertsAdjusted += _highProfitMessageSender.Flush();
        }

        private void HighVolume(IUniverseAlertEvent alert)
        {
            if (alert.IsDeleteEvent)
            {
                _highVolumeMessageSender.Delete();
                return;
            }

            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (IHighVolumeRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"high volume adding alert to high volume message sender");
            _highVolumeMessageSender.Send(ruleBreach);

            _logger.LogInformation($"high volume incrementing raw alert count by 1");
            Analytics.HighVolumeAlertsRaw += 1;
        }

        private void HighVolumeFlush()
        {
            _logger.LogInformation($"high volume flushing alerts");
            Analytics.HighVolumeAlertsAdjusted += _highVolumeMessageSender.Flush();
        }

        private void Layering(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (ILayeringRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"layering adding alert to layering message sender");
            _layeringCachedMessageSender.Send(ruleBreach);

            _logger.LogInformation($"layering incrementing raw alert count by 1");
            Analytics.LayeringAlertsRaw += 1;
        }

        private void LayeringFlush()
        {
            _logger.LogInformation($"layering flushing alerts");
            Analytics.LayeringAlertsAdjusted += _layeringCachedMessageSender.Flush();
        }
        
        private void MarkingTheClose(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (IMarkingTheCloseBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"marking the close adding alert to marking the close message sender");
            _markingTheCloseMessageSender.Send(ruleBreach);

            _logger.LogInformation($"marking the close incrementing raw and adjusted alert count by 1");
            Analytics.MarkingTheCloseAlertsRaw += 1;
            Analytics.MarkingTheCloseAlertsAdjusted += 1;
        }

        private void MarkingTheCloseFlush()
        {
            _logger.LogInformation($"marking the close flush event (does nothing) as it sends immediately");
        }

        private void Spoofing(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (ISpoofingRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"spoofing adding alert to spoofing message sender");
            _spoofingMessageSender.Send(ruleBreach);

            _logger.LogInformation($"spoofing incrementing raw and adjusted alert count by 1");
            Analytics.SpoofingAlertsRaw += 1;
            Analytics.SpoofingAlertsAdjusted += 1;
        }

        private void SpoofingFlush()
        {
            _logger.LogInformation($"spoofing flush event (does nothing) as it sends immediately");
        }

        private void EquityWashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"Equity wash trade adding alert to the wash trade message sender");
            _equityWashTradeMessageSender.Send(ruleBreach);

            _logger.LogInformation($"Equity wash trade incrementing raw alert count by 1");
            Analytics.WashTradeAlertsRaw += 1;
        }

        private void EquityWashTradeFlush()
        {
            _logger.LogInformation($"Equity wash trade flushing alerts");
            Analytics.WashTradeAlertsAdjusted += _equityWashTradeMessageSender.Flush();
        }

        private void FixedIncomeWashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent)
            {
                return;
            }

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            SetIsBackTest(ruleBreach);

            _logger.LogInformation($"Fixed income wash trade adding alert to the wash trade message sender");
            _fixedIncomeWashTradeMessageSender.Send(ruleBreach);

            _logger.LogInformation($"Fixed income wash trade incrementing raw alert count by 1");
            Analytics.FixedIncomeWashTradeAlertsRaw += 1;
        }

        private void FixedIncomeWashTradeFlush()
        {
            _logger.LogInformation($"Fixed income wash trade flushing alerts");
            Analytics.FixedIncomeWashTradeAlertsAdjusted += _fixedIncomeWashTradeMessageSender.Flush();
        }

        private void SetIsBackTest(IRuleBreach ruleBreach)
        {
            if (ruleBreach == null)
            {
                return;
            }

            ruleBreach.IsBackTestRun = _isBackTest;

            if (_isBackTest)
                _logger.LogInformation($"noting that alert is part of a back test and setting property");
        }

        public void Flush()
        {
            _logger?.LogInformation($"flush initiated.");

            EquityWashTradeFlush();
            FixedIncomeWashTradeFlush();
            
            SpoofingFlush();
            MarkingTheCloseFlush();
            LayeringFlush();

            HighVolumeFlush();
            HighProfitsFlush();

            CancelledOrdersFlush();

            _logger?.LogInformation($"flush completed.");
        }

        public AlertAnalytics Analytics { get; }
    }
}