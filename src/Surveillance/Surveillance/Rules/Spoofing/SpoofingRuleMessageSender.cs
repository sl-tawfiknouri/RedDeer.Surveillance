using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.SurveillanceService;
using Contracts.SurveillanceService.ComplianceCase;
using Contracts.SurveillanceService.ComplianceCaseLog;
using Domain.Trades.Orders;
using RedDeer.Contracts.SurveillanceService.TradeOrder;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Spoofing.Interfaces;
using Surveillance.Trades;
using Utilities.Extensions;

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
            var volumeInPosition = tradingPosition.VolumeInStatus(OrderStatus.Fulfilled);
            var volumeSpoofed = opposingPosition.VolumeNotInStatus(OrderStatus.Fulfilled);

            var description = $"Spoofing Rule Breach. Traded ({mostRecentTrade.Position.GetDescription()}) security {mostRecentTrade.Security?.Name} ({mostRecentTrade.Security?.Identifiers}) with a fulfilled trade volume of {volumeInPosition} and a cancelled trade volume of {volumeSpoofed}. The cancelled volume was traded in the opposite position to the most recent fulfilled trade and is therefore considered to be potential spoofing.";

            var caseDataItem = CaseDataItem(description, mostRecentTrade, tradingPosition, opposingPosition);
            var caseLogsInTradingPosition = CaseLogsInPosition(tradingPosition, true);
            var caseLogsAgainstTradingPosition = CaseLogsInPosition(opposingPosition, false);
            caseLogsInTradingPosition.AddRange(caseLogsAgainstTradingPosition);

            var caseMessage = new CaseMessage
            {
                Case = caseDataItem,
                CaseLogs = caseLogsInTradingPosition.ToArray()
            };

            _caseMessageSender.Send(caseMessage);
        }

        private static ComplianceCaseDataItemDto CaseDataItem(
            string description,
            TradeOrderFrame mostRecentTrade,
            TradePosition tradingPosition,
            TradePosition opposingPosition)
        {
            var earliestTp = tradingPosition.Get()?.Min(tp => tp.StatusChangedOn);
            var earliestOp = opposingPosition.Get()?.Min(op => op.StatusChangedOn);

            var from = earliestTp < earliestOp ? earliestTp : earliestOp;
            
            return new ComplianceCaseDataItemDto
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

        private List<ComplianceCaseLogDataItemDto> CaseLogsInPosition(TradePosition tradingPosition, bool executedPosition)
        {
            if (tradingPosition == null)
            {
                return new List<ComplianceCaseLogDataItemDto>();
            }

            return tradingPosition
                    .Get()
                    .Select(tp =>
                        new ComplianceCaseLogDataItemDto
                        {
                            Type = ComplianceCaseLogType.Unset,
                            Notes = ProjectCaseLog(executedPosition, tp),
                            UnderlyingOrder = Map(tp)
                        })
                    .ToList();
        }

        private TradeOrderDataItemDto Map(TradeOrderFrame frame)
        {
            if (frame == null)
            {
                return null;
            }

            return new TradeOrderDataItemDto
            {
                OrderType = (TradeOrderType)frame.OrderType,
                MarketIdentifierCode = frame.Market?.Id?.Id,
                MarketName = frame.Market?.Name,
                SecurityName = frame.Security?.Name,
                SecurityClientIdentifier = frame.Security?.Identifiers.ClientIdentifier,
                SecuritySedol = frame.Security?.Identifiers.Sedol,
                SecurityIsin = frame.Security?.Identifiers.Isin,
                SecurityFigi = frame.Security?.Identifiers.Figi,
                SecurityCusip = frame.Security?.Identifiers.Cusip,
                SecurityExchangeSymbol = frame.Security?.Identifiers.ExchangeSymbol,
                SecurityCfi = frame.Security?.Cfi,
                LimitPrice = frame.Limit?.Value,
                TradeSubmittedOn = frame.TradeSubmittedOn,
                StatusChangedOn = frame.StatusChangedOn,
                Volume = frame.Volume,
                OrderPosition = (TradeOrderPosition)frame.Position,
                OrderStatus = (TradeOrderStatus)frame.OrderStatus,
                TraderId = frame.TraderId,
                ClientAttributionId = frame.TradeClientAttributionId,
                PartyBrokerId = frame.PartyBrokerId,
                CounterPartyBrokerId = frame.CounterPartyBrokerId
            };
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

            return $"{preamble} {tof.Market?.Id.Id} ({tof.Market?.Name}) {tof.Security?.Name} ({tof.Security?.Identifiers.ToString()}) was traded  {tof.Position.GetDescription()} with order type {tof.OrderType}{limitSection} and volume {tof.Volume} order status last changed on {tof.StatusChangedOn}";
        }
    }
}
