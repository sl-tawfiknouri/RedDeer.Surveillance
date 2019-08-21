namespace Surveillance.Engine.Rules.Analytics.Subscriber.Factory
{
    using System;

    using Microsoft.Extensions.Logging;

    using Surveillance.Engine.Rules.Analytics.Subscriber.Factory.Interfaces;
    using Surveillance.Engine.Rules.Analytics.Subscriber.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.CancelledOrders.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.HighVolume.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Layering.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.PlacingOrderNoIntentToExecute.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Ramping.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.Spoofing.Interfaces;
    using Surveillance.Engine.Rules.Rules.Shared.WashTrade.Interfaces;

    public class UniverseAlertStreamSubscriberFactory : IUniverseAlertStreamSubscriberFactory
    {
        private readonly ICancelledOrderRuleCachedMessageSender _cancelledOrderMessageSender;

        private readonly IWashTradeCachedMessageSender _fixedIncomeWashTradeMessageSender;

        private readonly IHighVolumeRuleCachedMessageSender _highVolumeMessageSender;

        private readonly ILayeringCachedMessageSender _layeringCachedMessageSender;

        private readonly ILogger<IUniverseAlertSubscriber> _logger;

        private readonly IMarkingTheCloseMessageSender _markingTheCloseMessageSender;

        private readonly IPlacingOrdersWithNoIntentToExecuteCacheMessageSender _placingOrdersMessageSender;

        private readonly IRampingRuleCachedMessageSender _rampingMessageSender;

        private readonly ISpoofingRuleMessageSender _spoofingMessageSender;

        private readonly IWashTradeCachedMessageSender _washTradeMessageSender;

        public UniverseAlertStreamSubscriberFactory(
            ICancelledOrderRuleCachedMessageSender cancelledOrderMessageSender,
            IHighVolumeRuleCachedMessageSender highVolumeMessageSender,
            ILayeringCachedMessageSender layeringMessageSender,
            IMarkingTheCloseMessageSender markingTheCloseMessageSender,
            ISpoofingRuleMessageSender spoofingMessageSender,
            IWashTradeCachedMessageSender washTradeMessageSender,
            IWashTradeCachedMessageSender fixedIncomeWashTradeMessageSender,
            IRampingRuleCachedMessageSender rampingMessageSender,
            IPlacingOrdersWithNoIntentToExecuteCacheMessageSender placingOrdersMessageSender,
            ILogger<IUniverseAlertSubscriber> logger)
        {
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

            this._washTradeMessageSender =
                washTradeMessageSender ?? throw new ArgumentNullException(nameof(washTradeMessageSender));

            this._placingOrdersMessageSender = placingOrdersMessageSender
                                               ?? throw new ArgumentNullException(nameof(placingOrdersMessageSender));

            this._fixedIncomeWashTradeMessageSender = fixedIncomeWashTradeMessageSender
                                                      ?? throw new ArgumentNullException(
                                                          nameof(washTradeMessageSender));

            this._rampingMessageSender =
                rampingMessageSender ?? throw new ArgumentNullException(nameof(rampingMessageSender));

            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IUniverseAlertSubscriber Build(int opCtxId, bool isBackTest)
        {
            return new UniverseAlertsSubscriber(
                opCtxId,
                isBackTest,
                this._cancelledOrderMessageSender,
                this._highVolumeMessageSender,
                this._layeringCachedMessageSender,
                this._markingTheCloseMessageSender,
                this._spoofingMessageSender,
                this._washTradeMessageSender,
                this._fixedIncomeWashTradeMessageSender,
                this._rampingMessageSender,
                this._placingOrdersMessageSender,
                this._logger);
        }
    }
}