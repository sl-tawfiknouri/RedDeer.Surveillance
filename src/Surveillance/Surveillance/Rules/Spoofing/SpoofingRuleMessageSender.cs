using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Trades.Orders;
using MessageBusDtos.Surveillance;
using RedDeer.Contracts.DataItems.Compliance;
using RedDeer.Contracts.Enums;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;

namespace Surveillance.Rules.Spoofing
{
    public class SpoofingRuleMessageSender : ISpoofingRuleMessageSender
    {
        private readonly ICaseMessageSender _caseMessageSender;

        public SpoofingRuleMessageSender(ICaseMessageSender caseMessageSender)
        {
            _caseMessageSender = caseMessageSender ?? throw new ArgumentNullException(nameof(caseMessageSender));
        }

        public void Send(
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition,
            TradePosition opposingPosition)
        {
            var volumeInPosition = tradingPosition.VolumeInStatus(Domain.Trades.Orders.OrderStatus.Fulfilled);
            var volumeSpoofed = opposingPosition.VolumeNotInStatus(Domain.Trades.Orders.OrderStatus.Fulfilled);

            var description = $"Traded ({mostRecentTrade.Direction.ToString()}) {mostRecentTrade.Security?.Identifiers} with a fulfilled volume of {volumeInPosition} and a cancelled volume of {volumeSpoofed} in other trading direction preceding the most recent fulfilled trade.";

            var caseDataItem = CaseDataItem(description);
            var caseLogsInTradingPosition = CaseLogsInTradingPosition(tradingPosition);
            var caseLogsAgainstTradingPosition = CaseLogsAgainstTradingPosition(opposingPosition);
            caseLogsInTradingPosition.AddRange(caseLogsAgainstTradingPosition);

            var caseMessage = new CaseMessage
            {
                Case = caseDataItem,
                CaseLogs = caseLogsInTradingPosition.ToArray()
            };

            _caseMessageSender.Send(caseMessage);
        }

        private static ComplianceCaseDataItem CaseDataItem(string description)
        {
            return new ComplianceCaseDataItem
            {
                Title = "Automated Spoofing Rule Breach Detected",
                Description = description ?? string.Empty,
                Source = ComplianceCaseSource.SurveillanceRule,
                Status = ComplianceCaseStatus.BreachDetected,
                ReportedOn = DateTime.Now
            };
        }

        private List<ComplianceCaseLogDataItem> CaseLogsInTradingPosition(TradePosition tradingPosition)
        {
            if (tradingPosition == null)
            {
                return new List<ComplianceCaseLogDataItem>();
            }

            return tradingPosition
                    .Get()
                    .Select(tp =>
                        new ComplianceCaseLogDataItem
                        {
                            Type = ComplianceCaseLogType.Unset,
                            Notes = ProjectCaseLog(true, tp)
                        })
                    .ToList();
        }

        private List<ComplianceCaseLogDataItem> CaseLogsAgainstTradingPosition(TradePosition opposingPosition)
        {
            if (opposingPosition == null)
            {
                return new List<ComplianceCaseLogDataItem>();
            }

            return opposingPosition
                .Get()
                .Select(tp =>
                    new ComplianceCaseLogDataItem
                    {
                        Type = ComplianceCaseLogType.Unset,
                        Notes = ProjectCaseLog(false, tp)
                    })
                .ToList();
        }

        private string ProjectCaseLog(bool executedPosition, TradeOrderFrame tof)
        {
            if (tof == null)
            {
                return string.Empty;
            }

            var preamble =
                executedPosition
                    ? "Final trading position:"
                    : "Spoofed trading position:";

            var limitSection =
                tof.OrderType == OrderType.Limit
                    ? $" with limit of {tof.Limit?.Value}({tof.Limit?.Currency})"
                    : string.Empty;

            return $"{preamble} {tof.Market?.Id} ({tof.Market?.Name}) {tof.Security?.Name} ({tof.Security?.Identifiers.ToString()}) was traded  {tof.Direction} with order type {tof.OrderType}{limitSection} and volume {tof.Volume} order status last changed on{tof.StatusChangedOn}";
        }
    }
}
