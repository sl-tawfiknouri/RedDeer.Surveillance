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
using Surveillance.Rules.Interfaces;
using Surveillance.System.Auditing.Context.Interfaces;

namespace Surveillance.Rules
{
    public abstract class BaseMessageSender
    {
        private readonly ITradeOrderDataItemDtoMapper _dtoMapper;
        private readonly ICaseMessageSender _caseMessageSender;
        private readonly string _messageSenderName;
        private readonly string _caseTitle;
        private readonly ILogger _logger;

        protected BaseMessageSender(
            ITradeOrderDataItemDtoMapper dtoMapper,
            string caseTitle,
            string messageSenderName,
            ILogger logger,
            ICaseMessageSender caseMessageSender)
        {
            _dtoMapper = dtoMapper ?? throw new ArgumentNullException(nameof(dtoMapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _caseMessageSender = caseMessageSender ?? throw new ArgumentNullException(nameof(caseMessageSender));

            _messageSenderName = messageSenderName ?? "unknown message sender";
            _caseTitle = caseTitle ?? "unknown rule breach detected";
        }

        protected void Send(IRuleBreach ruleBreach, string description, ISystemProcessOperationRunRuleContext opCtx)
        {
            if (ruleBreach?.Trades?.Get() == null)
            {
                return;
            }

            description = description ?? string.Empty;

            var caseMessage = new CaseMessage
            {
                Case = CaseDataItem(ruleBreach, description),
                CaseLogs = CaseLogsInPosition(ruleBreach.Trades?.Get())
            };

            try
            {
                _caseMessageSender.Send(caseMessage);
            }
            catch (Exception e)
            {
                _logger.LogError($"{_messageSenderName} encountered an error sending the case message to the bus {e}");
                opCtx.EventException(e);
            }
        }

        private ComplianceCaseDataItemDto CaseDataItem(IRuleBreach ruleBreach, string description)
        {
            var oldestPosition = ruleBreach.Trades?.Get()?.Min(tr => tr.StatusChangedOn);
            var latestPosition = ruleBreach.Trades?.Get()?.Max(tr => tr.StatusChangedOn);
            var venue = ruleBreach.Trades?.Get()?.FirstOrDefault()?.Market?.Name;

            var entityReferences =
               string.IsNullOrWhiteSpace(ruleBreach.Security.Identifiers.ReddeerId)
                ? new EntityReference[0]
                : 
                new[]
                {
                    new EntityReference
                    {
                        EntityId = ruleBreach.Security.Identifiers.ReddeerId,
                        EntityType = EntityReferenceType.SecurityId
                    }
                };

            return new ComplianceCaseDataItemDto
            {
                Title = _caseTitle,
                Description = description,
                Source = ComplianceCaseSource.SurveillanceRule,
                Status = ComplianceCaseStatus.Unset,
                Type = ComplianceCaseType.Unset,
                ReportedOn = DateTime.Now,
                Venue = venue,
                StartOfPeriodUnderInvestigation = oldestPosition.GetValueOrDefault(DateTime.Now),
                EndOfPeriodUnderInvestigation = latestPosition.GetValueOrDefault(DateTime.Now),
                AssetType = ruleBreach.Security.Cfi,
                EntityReferences = entityReferences
            };
        }

        protected virtual ComplianceCaseLogDataItemDto[] CaseLogsInPosition(IList<TradeOrderFrame> orders)
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