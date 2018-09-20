using System;
using System.Linq;
using Contracts.SurveillanceService;
using Contracts.SurveillanceService.ComplianceCase;
using Contracts.SurveillanceService.ComplianceCaseLog;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.Cancelled_Orders.Interfaces;
using Surveillance.Rule_Parameters.Interfaces;
using Surveillance.Trades.Interfaces;
using Utilities.Extensions;

namespace Surveillance.Rules.Cancelled_Orders
{
    public class CancelledOrderMessageSender : ICancelledOrderMessageSender
    {
        private readonly ICaseMessageSender _sender;
        private readonly ITradeOrderDataItemDtoMapper _dtoMapper;
        private readonly ILogger _logger;

        public CancelledOrderMessageSender(
            ICaseMessageSender sender,
            ITradeOrderDataItemDtoMapper dtoMapper,
            ILogger<CancelledOrderMessageSender> logger)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _dtoMapper = dtoMapper ?? throw new ArgumentNullException(nameof(dtoMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Send(
            ITradePosition tradePosition,
            ICancelledOrderRuleBreach ruleBreach,
            ICancelledOrderRuleParameters parameters)
        {
            if (tradePosition == null
                || ruleBreach == null)
            {
                return;
            }

            var caseMessage = new CaseMessage
            {
                Case = CaseDataItem(tradePosition, ruleBreach, parameters),
                CaseLogs = CaseLogsInPosition(tradePosition)
            };

            try
            {
                _sender.Send(caseMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured in cancelled order message sender {e.Message}");
            }
        }

        private ComplianceCaseDataItemDto CaseDataItem(
            ITradePosition tradingPosition,
            ICancelledOrderRuleBreach ruleBreach,
            ICancelledOrderRuleParameters parameters)
        {
            var anyOrder = tradingPosition.Get().FirstOrDefault();
            var mostRecentTp = tradingPosition.Get()?.Max(tp => tp.StatusChangedOn);
            var oldestTp = tradingPosition.Get()?.Min(tp => tp.TradeSubmittedOn);
            var venue = anyOrder?.Market?.Name ?? string.Empty;
            var description = BuildDescription(parameters, ruleBreach, anyOrder);

            return new ComplianceCaseDataItemDto
            {
                Title = "Automated Cancellation Ratio Rule Breach Detected",
                Description = description,
                Source = ComplianceCaseSource.SurveillanceRule,
                Status = ComplianceCaseStatus.Unset,
                Type = ComplianceCaseType.Unset,
                ReportedOn = DateTime.Now,
                Venue = venue,
                StartOfPeriodUnderInvestigation = oldestTp.GetValueOrDefault(),
                EndOfPeriodUnderInvestigation = mostRecentTp.GetValueOrDefault(),
            };
        }

        private ComplianceCaseLogDataItemDto[] CaseLogsInPosition(ITradePosition tradingPosition)
        {
            if (tradingPosition == null
                || !tradingPosition.Get().Any())
            {
                return new ComplianceCaseLogDataItemDto[0];
            }

            return tradingPosition
                .Get()
                .Select(tp =>
                    new ComplianceCaseLogDataItemDto
                    {
                        Type = ComplianceCaseLogType.Unset,
                        Notes = string.Empty,
                        UnderlyingOrder = _dtoMapper.Map(tp)
                    })
                .ToArray();
        }

        private string BuildDescription(
            ICancelledOrderRuleParameters parameters,
            ICancelledOrderRuleBreach ruleBreach,
            Domain.Trades.Orders.TradeOrderFrame anyOrder)
        {
            var percentagePositionCancelled =
                Math.Round(
                    (ruleBreach.PercentagePositionCancelled.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var tradeCountCancelled =
                Math.Round(
                    (ruleBreach.PercentageTradeCountCancelled.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);


            var positionSizeSegment = ruleBreach.ExceededPercentagePositionCancellations
                ? $" Position cancelled exceeded threshold at {percentagePositionCancelled}% cancelled. Limit was set at {parameters.CancelledOrderPercentagePositionThreshold * 100}%."
                : string.Empty;

            var orderRatioSegment = ruleBreach.ExceededPercentageTradeCountCancellations
                ? $" Number of orders cancelled exceeded threshold at {tradeCountCancelled}% cancelled. Limit was set at {parameters.CancelledOrderCountPercentageThreshold * 100}%."
                : string.Empty;

            var description = $"Cancelled Order Rule Breach. Traded ({anyOrder?.Position.GetDescription()}) security {anyOrder?.Security?.Name} ({anyOrder?.Security?.Identifiers}) with excessive cancellations in {parameters.WindowSize.TotalMinutes} minute time period.{positionSizeSegment}{orderRatioSegment}";

            return description;
        }
    }
}