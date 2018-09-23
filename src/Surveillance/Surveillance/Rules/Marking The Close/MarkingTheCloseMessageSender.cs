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
            var dailyVolumePercentageSetByUser =
                Math.Round(
                    (ruleBreach.Parameters.PercentageThresholdDailyVolume.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailyBuyVolumePercentage =
                Math.Round(
                    (ruleBreach.BuyDailyVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailySellVolumePercentage =
                Math.Round(
                    (ruleBreach.SellDailyVolumeBreach.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var dailyBuyVolumeMark = BuyVolumeDescription(ruleBreach, dailyVolumePercentageSetByUser, dailyBuyVolumePercentage);
            var dailySellVolumeMark = SellVolumeDescription(ruleBreach, dailyVolumePercentageSetByUser, dailySellVolumePercentage);

            return $"Marking the close rule breach detected for {ruleBreach.Security} ({ruleBreach.Security.Identifiers}) traded on {ruleBreach.MarketClose.MarketId} which closed at {ruleBreach.MarketClose.MarketClose.ToShortTimeString()}. {dailyBuyVolumeMark}{dailySellVolumeMark}";
        }

        private string BuyVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal dailyVolumePercentageSetByUser,
            decimal dailyBuyVolumePercentage)
        {
            return ruleBreach.HasBuyDailyVolumeBreach
                ? $" Daily volume threshold of {dailyVolumePercentageSetByUser}% was exceeded on buy orders by {dailyBuyVolumePercentage}% of daily volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close."
                : string.Empty;
        }

        private string SellVolumeDescription(
            IMarkingTheCloseBreach ruleBreach,
            decimal dailyVolumePercentageSetByUser,
            decimal dailySellVolumePercentage)
        {
            return ruleBreach.HasSellDailyVolumeBreach
                ? $" Daily volume threshold of {dailyVolumePercentageSetByUser}% was exceeded on sell orders by {dailySellVolumePercentage}% of daily volume purchased within {ruleBreach.Window.TotalMinutes} minutes of market close."
                : string.Empty;
        }
    }
}
