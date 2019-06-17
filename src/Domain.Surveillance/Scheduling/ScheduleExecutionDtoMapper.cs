using System.Collections.Generic;
using System.Linq;
using Domain.Surveillance.Scheduling.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Surveillance.Scheduling
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
                Rules = rules,
                IsForceRerun = dto.IsForceRerun
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

        private Rules MapRule(RedDeer.Contracts.SurveillanceService.Rules.Rules rule)
        {
            switch (rule)
            {
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.CancelledOrders:
                    return Rules.CancelledOrders;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.HighProfits:
                    return Rules.HighProfits;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.HighVolume:
                    return Rules.HighVolume;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.Layering:
                    return Rules.Layering;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.MarkingTheClose:
                    return Rules.MarkingTheClose;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.Spoofing:
                    return Rules.Spoofing;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter:
                    return Rules.UniverseFilter;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.WashTrade:
                    return Rules.WashTrade;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.FrontRunning:
                    return Rules.FrontRunning;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.PaintingTheTape:
                    return Rules.PaintingTheTape;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.ImproperMatchedOrders:
                    return Rules.ImproperMatchedOrders;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.CrossAssetManipulation:
                    return Rules.CrossAssetManipulation;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.PumpAndDump:
                    return Rules.PumpAndDump;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.TrashAndCash:
                    return Rules.TrashAndCash;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeHighProfits:
                    return Rules.FixedIncomeHighProfits;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeHighVolumeIssuance:
                    return Rules.FixedIncomeHighVolumeIssuance;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeWashTrades:
                    return Rules.FixedIncomeWashTrades;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.Ramping:
                    return Rules.Ramping;
                case RedDeer.Contracts.SurveillanceService.Rules.Rules.PlacingOrdersWithNoIntentToExecute:
                    return Rules.PlacingOrderWithNoIntentToExecute;
            }

            _logger?.LogError($"ScheduleExecutionDtoMapper out of range for rule enum {rule} from contracts library");

            return Rules.UniverseFilter;
        }

        private RedDeer.Contracts.SurveillanceService.Rules.Rules MapRule(Rules rule)
        {
            switch (rule)
            {
                case Rules.CancelledOrders:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.CancelledOrders;
                case Rules.HighProfits:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.HighProfits;
                case Rules.HighVolume:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.HighVolume;
                case Rules.Layering:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.Layering;
                case Rules.MarkingTheClose:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.MarkingTheClose;
                case Rules.Spoofing:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.Spoofing;
                case Rules.UniverseFilter:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter;
                case Rules.WashTrade:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.WashTrade;
                case Rules.FrontRunning:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.FrontRunning;
                case Rules.PaintingTheTape:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.PaintingTheTape;
                case Rules.ImproperMatchedOrders:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.ImproperMatchedOrders;
                case Rules.CrossAssetManipulation:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.CrossAssetManipulation;
                case Rules.PumpAndDump:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.PumpAndDump;
                case Rules.TrashAndCash:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.TrashAndCash;
                case Rules.FixedIncomeHighProfits:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeHighProfits;
                case Rules.FixedIncomeHighVolumeIssuance:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeHighVolumeIssuance;
                case Rules.FixedIncomeWashTrades:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.FixedIncomeWashTrades;
                case Rules.Ramping:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.Ramping;
                case Rules.PlacingOrderWithNoIntentToExecute:
                    return RedDeer.Contracts.SurveillanceService.Rules.Rules.PlacingOrdersWithNoIntentToExecute;
            }

            _logger?.LogError($"ScheduleExecutionDtoMapper out of range for rule enum {rule} from domain library");

            return RedDeer.Contracts.SurveillanceService.Rules.Rules.UniverseFilter;
        }
    }
}
