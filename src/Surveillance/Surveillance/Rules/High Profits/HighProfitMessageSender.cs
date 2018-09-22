using System;
using System.Collections.Generic;
using System.Linq;
using Contracts.SurveillanceService;
using Contracts.SurveillanceService.ComplianceCase;
using Contracts.SurveillanceService.ComplianceCaseLog;
using Domain.Trades.Orders;
using Microsoft.Extensions.Logging;
using Surveillance.Mappers.Interfaces;
using Surveillance.MessageBus_IO.Interfaces;
using Surveillance.Rules.High_Profits.Interfaces;

namespace Surveillance.Rules.High_Profits
{
    public class HighProfitMessageSender : IHighProfitMessageSender
    {
        private readonly ICaseMessageSender _caseMessageSender;
        private readonly ITradeOrderDataItemDtoMapper _dtoMapper;
        private readonly ILogger<HighProfitMessageSender> _logger;

        public HighProfitMessageSender(
            ICaseMessageSender caseMessageSender,
            ILogger<HighProfitMessageSender> logger,
            ITradeOrderDataItemDtoMapper dtoMapper)
        {
            _caseMessageSender =
                caseMessageSender
                ?? throw new ArgumentNullException(nameof(caseMessageSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dtoMapper = dtoMapper ?? throw new ArgumentNullException(nameof(dtoMapper));
        }

        public void Send(IHighProfitRuleBreach ruleBreach)
        {
            if (ruleBreach?.Trades?.Get() == null
                || ruleBreach?.Trades?.Get().Count < 2)
            {
                return;
            }

            var caseMessage = new CaseMessage
            {
                Case = CaseDataItem(ruleBreach),
                CaseLogs = CaseLogsInPosition(ruleBreach.Trades?.Get())
            };

            try
            {
                _caseMessageSender.Send(caseMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"High Profit Message Sender encountered an error sending the case message to the bus {e}");
            }
        }

        private ComplianceCaseDataItemDto CaseDataItem(IHighProfitRuleBreach ruleBreach)
        {
            var oldestPosition = ruleBreach.Trades?.Get()?.Min(tr => tr.StatusChangedOn);
            var latestPosition = ruleBreach.Trades?.Get()?.Max(tr => tr.StatusChangedOn);
            var venue = ruleBreach.Trades?.Get()?.FirstOrDefault()?.Market?.Name;

            var description = BuildDescription(ruleBreach);

            return new ComplianceCaseDataItemDto
            {
                Title = "Automated High Profit Rule Breach Detected",
                Description = description,
                Source = ComplianceCaseSource.SurveillanceRule,
                Status = ComplianceCaseStatus.Unset,
                Type = ComplianceCaseType.Unset,
                ReportedOn = DateTime.Now,
                Venue = venue,
                StartOfPeriodUnderInvestigation = oldestPosition.GetValueOrDefault(DateTime.Now),
                EndOfPeriodUnderInvestigation = latestPosition.GetValueOrDefault(DateTime.Now),
            };
        }

        private string BuildDescription(IHighProfitRuleBreach ruleBreach)
        {
            var highRelativeProfitAsPercentage =
                Math.Round(
                    (ruleBreach.RelativeProfits.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var highRelativeProfitAsPercentageSetByUser =
                Math.Round(
                    (ruleBreach.Parameters.HighProfitPercentageThreshold.GetValueOrDefault(0) * 100m),
                    2,
                    MidpointRounding.AwayFromZero);

            var highRelativeProfitSection =
                ruleBreach.HasRelativeProfitBreach
                    ? $" There was a high profit ratio of {highRelativeProfitAsPercentage}% which exceeded the configured high profit ratio percentage threshold of {highRelativeProfitAsPercentageSetByUser}%."
                    : string.Empty;

            var highAbsoluteProfitSection =
                ruleBreach.HasAbsoluteProfitBreach
                    ? $" There was a high profit of {ruleBreach.AbsoluteProfits} ({ruleBreach.AbsoluteProfitCurrency}) which exceeded the configured profit limit of {ruleBreach.Parameters.HighProfitAbsoluteThreshold.GetValueOrDefault(0)}({ruleBreach.Parameters.HighProfitAbsoluteThresholdCurrency})."
                    : string.Empty;

            return $"High profit rule breach detected for {ruleBreach.Security.Name} ({ruleBreach.Security.Identifiers}).{highRelativeProfitSection}{highAbsoluteProfitSection}";
        }

        private ComplianceCaseLogDataItemDto[] CaseLogsInPosition(IList<TradeOrderFrame> orders)
        {
            if (orders == null
                || !orders.Any())
            {
                return new ComplianceCaseLogDataItemDto[0];
            }

            return orders
                .Select(tp =>
                    new ComplianceCaseLogDataItemDto
                    {
                        Type = ComplianceCaseLogType.Unset,
                        Notes = string.Empty,
                        UnderlyingOrder = _dtoMapper.Map(tp)
                    })
                .ToArray();
        }
    }
}