using System.Collections.Generic;
using System.Linq;
using DomainV2.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;

namespace DomainV2.Scheduling
{
    public class ScheduleExecutionDtoMapper : IScheduleExecutionDtoMapper
    {
        private readonly ILogger<ScheduleExecutionDtoMapper> _logger;

        public ScheduleExecutionDtoMapper(ILogger<ScheduleExecutionDtoMapper> logger)
        {
            _logger = logger;
        }

        public ScheduledExecution MapToDomain(RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution dto)
        {
            if (dto == null)
            {
                return new ScheduledExecution();
            }

            var rules =
                dto
                    .Rules
                    ?.Select(ru =>
                        new RuleIdentifier
                        {
                            Ids = ru.Ids,
                            Rule = MapRule(ru.Rule)
                        })
                    .ToList()
                ?? new List<RuleIdentifier>();

            var response = new ScheduledExecution
            {
                CorrelationId = dto.CorrelationId,
                IsBackTest = dto.IsBackTest,
                TimeSeriesInitiation = dto.TimeSeriesInitiation,
                TimeSeriesTermination = dto.TimeSeriesTermination,
                Rules = rules
            };

            return response;
        }

        public RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution MapToDto(ScheduledExecution dto)
        {
            if (dto == null)
            {
                return new RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution();
            }

            var rules =
                dto
                    .Rules
                    ?.Select(ru => 
                        new RedDeer.Contracts.SurveillanceService.Rules.RuleIdentifier
                        {
                            Ids = ru.Ids,
                            Rule = MapRule(ru.Rule)
                        }).ToList()
                ?? new List<RedDeer.Contracts.SurveillanceService.Rules.RuleIdentifier>();

            var scheduleExecution = new RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution
            {
                CorrelationId = dto.CorrelationId,
                IsBackTest = dto.IsBackTest,
                TimeSeriesInitiation = dto.TimeSeriesInitiation,
                TimeSeriesTermination = dto.TimeSeriesTermination,
                Rules = rules,
                IsForceRerun = dto.IsForceRerun
            };

            return scheduleExecution;
        }

        private DomainV2.Scheduling.Rules MapRule(RedDeer.Contracts.SurveillanceService.Rules.Rules rule)
        {
            switch (rule)
            {
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.CancelledOrders:
                    return DomainV2.Scheduling.Rules.CancelledOrders;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.HighProfits:
                    return DomainV2.Scheduling.Rules.HighProfits;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.HighVolume:
                    return DomainV2.Scheduling.Rules.HighVolume;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.Layering:
                    return DomainV2.Scheduling.Rules.Layering;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.MarkingTheClose:
                    return DomainV2.Scheduling.Rules.MarkingTheClose;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.Spoofing:
                    return DomainV2.Scheduling.Rules.Spoofing;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter:
                    return DomainV2.Scheduling.Rules.UniverseFilter;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.WashTrade:
                    return DomainV2.Scheduling.Rules.WashTrade;
            }

            _logger?.LogError($"ScheduleExecutionDtoMapper out of range for rule enum {rule} from contracts library");

            return DomainV2.Scheduling.Rules.UniverseFilter;
        }

        private RedDeer.Contracts.SurveillanceService.Rules.Rules MapRule(DomainV2.Scheduling.Rules rule)
        {
            switch (rule)
            {
                case DomainV2.Scheduling.Rules.CancelledOrders:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.CancelledOrders;
                case DomainV2.Scheduling.Rules.HighProfits:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.HighProfits;
                case DomainV2.Scheduling.Rules.HighVolume:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.HighVolume;
                case DomainV2.Scheduling.Rules.Layering:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.Layering;
                case DomainV2.Scheduling.Rules.MarkingTheClose:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.MarkingTheClose;
                case DomainV2.Scheduling.Rules.Spoofing:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.Spoofing;
                case DomainV2.Scheduling.Rules.UniverseFilter:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter;
                case DomainV2.Scheduling.Rules.WashTrade:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.WashTrade;
            }

            _logger?.LogError($"ScheduleExecutionDtoMapper out of range for rule enum {rule} from domain library");

            return RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter;
        }
    }
}
