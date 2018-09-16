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

            var description = $"Spoofing Rule Breach. Traded ({mostRecentTrade.Position.ToString()}) security {mostRecentTrade.Security?.Name} ({mostRecentTrade.Security?.Identifiers}) with a fulfilled trade volume of {volumeInPosition} and a cancelled trade volume of {volumeSpoofed}. The cancelled volume was traded in the opposite position to the most recent fulfilled trade and is therefore considered to be potential spoofing.";

            var caseDataItem = CaseDataItem(description, mostRecentTrade, tradingPosition, opposingPosition);
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

        private static ComplianceCaseDataItem CaseDataItem(
            string description,
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition,
            TradePosition opposingPosition)
        {
            var earliestTp = tradingPosition.Get()?.Min(tp => tp.StatusChangedOn);
            var earliestOp = opposingPosition.Get()?.Min(op => op.StatusChangedOn);

            var from = earliestTp < earliestOp ? earliestTp : earliestOp;
            
            return new ComplianceCaseDataItem
            {
                Title = "Automated Spoofing Rule Breach Detected",
                Description = description ?? string.Empty,
                Source = ComplianceCaseSource.SurveillanceRule,
                Status = ComplianceCaseStatus.Unset,
                Type = ComplianceCaseType.Unset,
                ReportedOn = DateTime.Now,
                Venue = mostRecentTrade.Market?.Name,
                StartOfPeriodUnderInvestigation = from.GetValueOrDefault(mostRecentTrade.StatusChangedOn),
                EndOfPeriodUnderInvestigation = mostRecentTrade.StatusChangedOn
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
                    ? "Executed trading position:"
                    : "Spoofed trading position:";

            var limitSection =
                tof.OrderType == OrderType.Limit
                    ? $" with limit of {tof.Limit?.Value}({tof.Limit?.Currency})"
                    : string.Empty;

            return $"{preamble} {tof.Market?.Id.Id} ({tof.Market?.Name}) {tof.Security?.Name} ({tof.Security?.Identifiers.ToString()}) was traded  {tof.Position} with order type {tof.OrderType}{limitSection} and volume {tof.Volume} order status last changed on {tof.StatusChangedOn}";
        }
    }
}
