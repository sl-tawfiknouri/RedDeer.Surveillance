﻿namespace Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Surveillance.DataLayer.Aurora.Rules.Interfaces;
    using Surveillance.Engine.Rules.Mappers.RuleBreach.Interfaces;
    using Surveillance.Engine.Rules.Queues.Interfaces;
    using Surveillance.Engine.Rules.Rules.Equity.MarkingTheClose.Interfaces;

    public class MarkingTheCloseMessageSender : BaseMessageSender, IMarkingTheCloseMessageSender
    {
        public MarkingTheCloseMessageSender(
            ILogger<MarkingTheCloseMessageSender> logger,
            IQueueCasePublisher queueCasePublisher,
            IRuleBreachRepository repository,
            IRuleBreachOrdersRepository ordersRepository,
            IRuleBreachToRuleBreachOrdersMapper ruleBreachToRuleBreachOrdersMapper,
            IRuleBreachToRuleBreachMapper ruleBreachToRuleBreachMapper)
            : base(
                "Automated Marking The Close Rule Breach Detected",
                "Marking The Close Message Sender",
                logger,
                queueCasePublisher,
                repository,
                ordersRepository,
                ruleBreachToRuleBreachOrdersMapper,
                ruleBreachToRuleBreachMapper)
        {
        }

        public async Task Send(IMarkingTheCloseBreach breach)
        {
            if (breach == null)
            {
                this.Logger.LogInformation(
                    "MarkingTheCloseMessageSender received a null breach for rule ctx. Returning.");
                return;
            }

            var description = this.BuildDescription(breach);
            await this.Send(breach, description);
        }

        private string AppendDailyBreach(IMarkingTheCloseBreach ruleBreach, string header)
        {
            if (ruleBreach?.DailyBreach == null) return header;

            var dailyVolumePercentageSetByUser = Math.Round(
                ruleBreach.EquitiesParameters.PercentageThresholdDailyVolume.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var dailyBuyVolumePercentage = Math.Round(
                ruleBreach.DailyBreach.BuyVolumeBreach.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var dailySellVolumePercentage = Math.Round(
                ruleBreach.DailyBreach.SellVolumeBreach.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var dailyBuyVolumeMark = ruleBreach.DailyBreach.HasBuyVolumeBreach
                                         ? this.BuyVolumeDescription(
                                             ruleBreach,
                                             dailyVolumePercentageSetByUser,
                                             dailyBuyVolumePercentage,
                                             true)
                                         : string.Empty;

            var dailySellVolumeMark = ruleBreach.DailyBreach.HasSellVolumeBreach
                                          ? this.SellVolumeDescription(
                                              ruleBreach,
                                              dailyVolumePercentageSetByUser,
                                              dailySellVolumePercentage,
                                              true)
                                          : string.Empty;

            header = $"{header}{dailyBuyVolumeMark}{dailySellVolumeMark}";

            return header;
        }

        private string AppendWindowBreach(IMarkingTheCloseBreach ruleBreach, string header)
        {
            if (ruleBreach?.WindowBreach == null) return header;

            var windowVolumePercentageSetByUser = Math.Round(
                ruleBreach.EquitiesParameters.PercentageThresholdWindowVolume.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var windowBuyVolumePercentage = Math.Round(
                ruleBreach.WindowBreach.BuyVolumeBreach.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var windowSellVolumePercentage = Math.Round(
                ruleBreach.WindowBreach.SellVolumeBreach.GetValueOrDefault(0) * 100m,
                2,
                MidpointRounding.AwayFromZero);

            var windowBuyVolumeMark = ruleBreach.WindowBreach.HasBuyVolumeBreach
                                          ? this.BuyVolumeDescription(
                                              ruleBreach,
                                              windowVolumePercentageSetByUser,
                                              windowBuyVolumePercentage,
                                              false)
                                          : string.Empty;

            var windowSellVolumeMark = ruleBreach.WindowBreach.HasSellVolumeBreach
                                           ? this.SellVolumeDescription(
                                               ruleBreach,
                                               windowVolumePercentageSetByUser,
                                               windowSellVolumePercentage,
                                               false)
                                           : string.Empty;

            header = $"{header}{windowBuyVolumeMark}{windowSellVolumeMark}";

            return header;
        }

        private string BuildDescription(IMarkingTheCloseBreach ruleBreach)
        {
            var header =
                $"Marking the close rule breach detected for {ruleBreach.Security.Name} traded on {ruleBreach.MarketClose.MarketId} which closed at {ruleBreach.MarketClose.MarketClose.ToShortTimeString()}. ";

            if ((ruleBreach.WindowBreach?.HasBuyVolumeBreach ?? false)
                || (ruleBreach.WindowBreach?.HasSellVolumeBreach ?? false))
                header = this.AppendWindowBreach(ruleBreach, header);

            if ((ruleBreach.DailyBreach?.HasBuyVolumeBreach ?? false)
                || (ruleBreach.DailyBreach?.HasSellVolumeBreach ?? false))
                header = this.AppendDailyBreach(ruleBreach, header);

            return header;
        }

        private string BuyVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal volumePercentageSetByUser,
            decimal buyVolumePercentage,
            bool window)
        {
            var windowType = window ? "Daily" : "Window";

            return
                $" {windowType} volume threshold of {volumePercentageSetByUser}% was exceeded on buy orders by {buyVolumePercentage}% of {windowType.ToLower()} volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close.";
        }

        private string SellVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal volumePercentageSetByUser,
            decimal sellVolumePercentage,
            bool window)
        {
            var windowType = window ? "Daily" : "Window";

            return
                $" {windowType} volume threshold of {volumePercentageSetByUser}% was exceeded on sell orders by {sellVolumePercentage}% of {windowType.ToLower()} volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close.";
        }
    }
}