namespace Domain.Scheduling.Interfaces
{
    public interface IScheduleExecutionDtoMapper
    {
        ScheduledExecution MapToDomain(RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution dto);
        RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution MapToDto(ScheduledExecution dto);
    }
}