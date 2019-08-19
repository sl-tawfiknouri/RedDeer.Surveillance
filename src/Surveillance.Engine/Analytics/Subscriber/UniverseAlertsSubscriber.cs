namespace Surveillance.Engine.Rules.Analytics.Subscriber
{
    using System;

    using Domain.Surveillance.Scheduling;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Analytics;
    using Surveillance.Engine.Rules.Analytics.Streams.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
    using Surveillance.Engine.Rules.Rules.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;

    /// <summary>
    ///     This is primarily for cross-rule analytics
    ///     In the future it will be used for calculating correlations between underlying positions
    ///     And perhaps aggregating alerts from multiple rules
    ///     We share one stream per scheduled execution so we can perform cross analysis on alerts raised
    /// </summary>
    public class UniverseAlertsSubscriber : IUniverseAlertSubscriber
    {
        private readonly ICancelledOrderRuleCachedMessageSender _cancelledOrderMessageSender;

        private readonly IWashTradeCachedMessageSender _equityWashTradeMessageSender;

        private readonly IWashTradeCachedMessageSender _fixedIncomeWashTradeMessageSender;

        private readonly IHighVolumeRuleCachedMessageSender _highVolumeMessageSender;

        private readonly bool _isBackTest;

        private readonly ILayeringCachedMessageSender _layeringCachedMessageSender;

        private readonly ILogger<IUniverseAlertSubscriber> _logger;

        private readonly IMarkingTheCloseMessageSender _markingTheCloseMessageSender;

        private readonly IPlacingOrdersWithNoIntentToExecuteCacheMessageSender _placingOrdersMessageSender;

        private readonly IRampingRuleCachedMessageSender _rampingRuleMessageSender;

        private readonly ISpoofingRuleMessageSender _spoofingMessageSender;

        public UniverseAlertsSubscriber(
            int opCtxId,
            bool isBackTest,
            ICancelledOrderRuleCachedMessageSender cancelledOrderMessageSender,
            IHighVolumeRuleCachedMessageSender highVolumeMessageSender,
            ILayeringCachedMessageSender layeringMessageSender,
            IMarkingTheCloseMessageSender markingTheCloseMessageSender,
            ISpoofingRuleMessageSender spoofingMessageSender,
            IWashTradeCachedMessageSender equityWashTradeMessageSender,
            IWashTradeCachedMessageSender fixedIncomeWashTradeMessageSender,
            IRampingRuleCachedMessageSender rampingRuleMessageSender,
            IPlacingOrdersWithNoIntentToExecuteCacheMessageSender placingOrdersCacheMessageSender,
            ILogger<IUniverseAlertSubscriber> logger)
        {
            this._isBackTest = isBackTest;

            this._cancelledOrderMessageSender = cancelledOrderMessageSender
                                                ?? throw new ArgumentNullException(nameof(cancelledOrderMessageSender));

            this._highVolumeMessageSender = highVolumeMessageSender
                                            ?? throw new ArgumentNullException(nameof(highVolumeMessageSender));

            this._layeringCachedMessageSender =
                layeringMessageSender ?? throw new ArgumentNullException(nameof(layeringMessageSender));

            this._markingTheCloseMessageSender = markingTheCloseMessageSender
                                                 ?? throw new ArgumentNullException(
                                                     nameof(markingTheCloseMessageSender));

            this._spoofingMessageSender =
                spoofingMessageSender ?? throw new ArgumentNullException(nameof(spoofingMessageSender));

            this._equityWashTradeMessageSender = equityWashTradeMessageSender
                                                 ?? throw new ArgumentNullException(
                                                     nameof(equityWashTradeMessageSender));

            this._fixedIncomeWashTradeMessageSender = fixedIncomeWashTradeMessageSender
                                                      ?? throw new ArgumentNullException(
                                                          nameof(fixedIncomeWashTradeMessageSender));

            this._rampingRuleMessageSender = rampingRuleMessageSender
                                             ?? throw new ArgumentNullException(nameof(rampingRuleMessageSender));

            this._placingOrdersMessageSender = placingOrdersCacheMessageSender
                                               ?? throw new ArgumentNullException(
                                                   nameof(placingOrdersCacheMessageSender));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.Analytics = new AlertAnalytics { SystemProcessOperationId = opCtxId };
        }

        public AlertAnalytics Analytics { get; }

        public void Flush()
        {
            this._logger?.LogInformation("flush initiated.");

            this.EquityWashTradeFlush();
            this.FixedIncomeWashTradeFlush();

            this.SpoofingFlush();
            this.MarkingTheCloseFlush();
            this.LayeringFlush();

            this.HighVolumeFlush();

            this.RampingFlush();
            this.CancelledOrdersFlush();
            this.PlacingOrdersWithoutIntentToExecuteFlush();

            this._logger?.LogInformation("flush completed.");
        }

        public void OnCompleted()
        {
            this._logger.LogInformation("received the OnCompleted() event from the stream");
        }

        public void OnError(Exception error)
        {
            this._logger.LogError(
                $"received the OnError() event from the stream {error.Message} {error.InnerException?.Message}");
        }

        public void OnNext(IUniverseAlertEvent value)
        {
            this._logger.LogInformation(
                $"received on the OnNext() event from the alerts stream. Rule {value.Rule} operation context {value.Context?.Id()}");

            switch (value.Rule)
            {
                case Rules.CancelledOrders:
                    this.CancelledOrders(value);
                    break;
                case Rules.HighVolume:
                    this.HighVolume(value);
                    break;
                case Rules.Layering:
                    this.Layering(value);
                    break;
                case Rules.MarkingTheClose:
                    this.MarkingTheClose(value);
                    break;
                case Rules.Spoofing:
                    this.Spoofing(value);
                    break;
                case Rules.WashTrade:
                    this.EquityWashTrade(value);
                    break;
                case Rules.FixedIncomeWashTrades:
                    this.FixedIncomeWashTrade(value);
                    break;
                case Rules.Ramping:
                    this.Ramping(value);
                    break;
                case Rules.PlacingOrderWithNoIntentToExecute:
                    this.PlacingOrdersWithoutIntentToExecute(value);
                    break;
                default:
                    this._logger.LogError(
                        $"met a rule type it did not identify {value.Rule}. This should be explicitly addressed.");
                    break;
            }
        }

        private void CancelledOrders(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (ICancelledOrderRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("cancelled orders adding alert to cancelled order message sender");
            this._cancelledOrderMessageSender.Send(ruleBreach);

            this._logger.LogInformation("cancelled orders incrementing raw alert count by 1");
            this.Analytics.CancelledOrderAlertsRaw += 1;
        }

        private void CancelledOrdersFlush()
        {
            this._logger.LogInformation("cancelled orders flushing alerts");
            this.Analytics.CancelledOrderAlertsAdjusted += this._cancelledOrderMessageSender.Flush();
        }

        private void EquityWashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("Equity wash trade adding alert to the wash trade message sender");
            this._equityWashTradeMessageSender.Send(ruleBreach);

            this._logger.LogInformation("Equity wash trade incrementing raw alert count by 1");
            this.Analytics.WashTradeAlertsRaw += 1;
        }

        private void EquityWashTradeFlush()
        {
            this._logger.LogInformation("Equity wash trade flushing alerts");
            this.Analytics.WashTradeAlertsAdjusted += this._equityWashTradeMessageSender.Flush();
        }

        private void FixedIncomeWashTrade(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (IWashTradeRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("Fixed income wash trade adding alert to the wash trade message sender");
            this._fixedIncomeWashTradeMessageSender.Send(ruleBreach);

            this._logger.LogInformation("Fixed income wash trade incrementing raw alert count by 1");
            this.Analytics.FixedIncomeWashTradeAlertsRaw += 1;
        }

        private void FixedIncomeWashTradeFlush()
        {
            this._logger.LogInformation("Fixed income wash trade flushing alerts");
            this.Analytics.FixedIncomeWashTradeAlertsAdjusted += this._fixedIncomeWashTradeMessageSender.Flush();
        }

        private void HighVolume(IUniverseAlertEvent alert)
        {
            if (alert.IsDeleteEvent)
            {
                this._highVolumeMessageSender.Delete();
                return;
            }

            if (alert.IsFlushEvent) return;

            var ruleBreach = (IHighVolumeRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("high volume adding alert to high volume message sender");
            this._highVolumeMessageSender.Send(ruleBreach);

            this._logger.LogInformation("high volume incrementing raw alert count by 1");
            this.Analytics.HighVolumeAlertsRaw += 1;
        }

        private void HighVolumeFlush()
        {
            this._logger.LogInformation("high volume flushing alerts");
            this.Analytics.HighVolumeAlertsAdjusted += this._highVolumeMessageSender.Flush();
        }

        private void Layering(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (ILayeringRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("layering adding alert to layering message sender");
            this._layeringCachedMessageSender.Send(ruleBreach);

            this._logger.LogInformation("layering incrementing raw alert count by 1");
            this.Analytics.LayeringAlertsRaw += 1;
        }

        private void LayeringFlush()
        {
            this._logger.LogInformation("layering flushing alerts");
            this.Analytics.LayeringAlertsAdjusted += this._layeringCachedMessageSender.Flush();
        }

        private void MarkingTheClose(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (IMarkingTheCloseBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("marking the close adding alert to marking the close message sender");
            this._markingTheCloseMessageSender.Send(ruleBreach);

            this._logger.LogInformation("marking the close incrementing raw and adjusted alert count by 1");
            this.Analytics.MarkingTheCloseAlertsRaw += 1;
            this.Analytics.MarkingTheCloseAlertsAdjusted += 1;
        }

        private void MarkingTheCloseFlush()
        {
            this._logger.LogInformation("marking the close flush event (does nothing) as it sends immediately");
        }

        private void PlacingOrdersWithoutIntentToExecute(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (IPlacingOrdersWithNoIntentToExecuteRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("adding alert to placing orders without intent to execute message sender");
            this._placingOrdersMessageSender.Send(ruleBreach);

            this._logger.LogInformation("placing orders without intent to execute incrementing raw alert count by 1");
            this.Analytics.PlacingOrdersAlertsRaw += 1;
        }

        private void PlacingOrdersWithoutIntentToExecuteFlush()
        {
            this._logger.LogInformation("placing orders without intent to execute flushing alerts");
            this.Analytics.PlacingOrdersAlertsAdjusted = this._placingOrdersMessageSender.Flush();
        }

        private void Ramping(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (IRampingRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("ramping adding alert to ramping message sender");
            this._rampingRuleMessageSender.Send(ruleBreach);

            this._logger.LogInformation("ramping incrementing raw alert count by 1");
            this.Analytics.RampingAlertsRaw += 1;
        }

        private void RampingFlush()
        {
            this._logger.LogInformation("ramping flushing alerts");
            this.Analytics.RampingAlertsAdjusted += this._rampingRuleMessageSender.Flush();
        }

        private void SetIsBackTest(IRuleBreach ruleBreach)
        {
            if (ruleBreach == null) return;

            ruleBreach.IsBackTestRun = this._isBackTest;

            if (this._isBackTest)
                this._logger.LogInformation("noting that alert is part of a back test and setting property");
        }

        private void Spoofing(IUniverseAlertEvent alert)
        {
            if (alert.IsFlushEvent) return;

            var ruleBreach = (ISpoofingRuleBreach)alert.UnderlyingAlert;
            this.SetIsBackTest(ruleBreach);

            this._logger.LogInformation("spoofing adding alert to spoofing message sender");
            this._spoofingMessageSender.Send(ruleBreach);

            this._logger.LogInformation("spoofing incrementing raw and adjusted alert count by 1");
            this.Analytics.SpoofingAlertsRaw += 1;
            this.Analytics.SpoofingAlertsAdjusted += 1;
        }

        private void SpoofingFlush()
        {
            this._logger.LogInformation("spoofing flush event (does nothing) as it sends immediately");
        }
    }
}