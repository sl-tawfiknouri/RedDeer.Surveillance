namespace DomainV2.Scheduling.Interfaces
{
    public interface IScheduleExecutionDtoMapper
    {
        DomainV2.Scheduling.ScheduledExecution MapToDomain(RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution dto);
        RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution MapToDto(DomainV2.Scheduling.ScheduledExecution dto);
    }
}