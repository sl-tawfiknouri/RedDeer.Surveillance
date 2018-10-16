using System;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Marking_The_Close.Interfaces;

namespace Surveillance.Rules.Marking_The_Close
{
    public class MarkingTheCloseMessageSender : BaseMessageSender, IMarkingTheCloseMessageSender
    {
        public MarkingTheCloseMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<MarkingTheCloseMessageSender> logger, 
            ICaseMessageSender caseMessageSender)
            : base(
                dtoMapper,
                "Automated Marking The Close Rule Breach Detected",
                "Marking The Close Message Sender",
                logger,
                caseMessageSender)
        { }

        public void Send(IMarkingTheCloseBreach breach)
        {
            if (breach == null)
            {
                return;
            }

            var description = BuildDescription(breach);
            Send(breach, description);
        }

        private string BuildDescription(IMarkingTheCloseBreach ruleBreach)
        {
            var header =
                $"Marking the close rule breach detected for {ruleBreach.Security} ({ruleBreach.Security.Identifiers}) traded on {ruleBreach.MarketClose.MarketId} which closed at {ruleBreach.MarketClose.MarketClose.ToShortTimeString()}. ";

            if ((ruleBreach.DailyBreach?.HasBuyVolumeBreach ?? false)
                || (ruleBreach.DailyBreach?.HasSellVolumeBreach ?? false))
            {
                header = AppendDailyBreach(ruleBreach, header);
            }

            if ((ruleBreach.WindowBreach?.HasBuyVolumeBreach ?? false)
                || (ruleBreach.WindowBreach?.HasSellVolumeBreach ?? false))
            {
                header = AppendWindowBreach(ruleBreach, header);
            }

            return header;
        }

        private string AppendWindowBreach(IMarkingTheCloseBreach ruleBreach, string header)
        {
            if (ruleBreach?.WindowBreach == null)
            {
                return header;
            }

            var windowVolumePercentageSetByUser =
                Math.Round(
                    (ruleBreach.Parameters.PercentageThresholdWindowVolume.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var windowBuyVolumePercentage =
                Math.Round(
                    (ruleBreach.WindowBreach.BuyVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var windowSellVolumePercentage =
                Math.Round(
                    (ruleBreach.WindowBreach.SellVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var windowBuyVolumeMark =
                ruleBreach?.WindowBreach?.HasBuyVolumeBreach ?? false
                ? BuyVolumeDescription(ruleBreach, windowVolumePercentageSetByUser, windowBuyVolumePercentage, false)
                : string.Empty;

            var windowSellVolumeMark =
                ruleBreach?.WindowBreach?.HasSellVolumeBreach ?? false
                ? SellVolumeDescription(ruleBreach, windowVolumePercentageSetByUser, windowSellVolumePercentage, false)
                : string.Empty;

            header = $"{header}{windowBuyVolumeMark}{windowSellVolumeMark}";

            return header;
        }

        private string AppendDailyBreach(IMarkingTheCloseBreach ruleBreach, string header)
        {
            if (ruleBreach?.DailyBreach == null)
            {
                return header;
            }

            var dailyVolumePercentageSetByUser =
                Math.Round(
                    (ruleBreach.Parameters.PercentageThresholdDailyVolume.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailyBuyVolumePercentage =
                Math.Round(
                    (ruleBreach.DailyBreach.BuyVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailySellVolumePercentage =
                Math.Round(
                    (ruleBreach.DailyBreach.SellVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailyBuyVolumeMark =
                ruleBreach?.DailyBreach?.HasBuyVolumeBreach ?? false
                ? BuyVolumeDescription(ruleBreach, dailyVolumePercentageSetByUser, dailyBuyVolumePercentage, true)
                : string.Empty;

            var dailySellVolumeMark =
                ruleBreach?.DailyBreach?.HasSellVolumeBreach ?? false
                ? SellVolumeDescription(ruleBreach, dailyVolumePercentageSetByUser, dailySellVolumePercentage, true)
                : string.Empty;

            header = $"{header}{dailyBuyVolumeMark}{dailySellVolumeMark}";

            return header;
        }

        private string BuyVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal volumePercentageSetByUser,
            decimal buyVolumePercentage,
            bool window)
        {
            var windowType = window ? "daily" : "window";

            return $" {windowType} volume threshold of {volumePercentageSetByUser}% was exceeded on buy orders by {buyVolumePercentage}% of {windowType} volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close.";
        }

        private string SellVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal volumePercentageSetByUser,
            decimal sellVolumePercentage,
            bool window)
        {
            var windowType = window ? "daily" : "window";

            return $" {windowType} volume threshold of {volumePercentageSetByUser}% was exceeded on sell orders by {sellVolumePercentage}% of {windowType} volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close.";
        }
    }
}
