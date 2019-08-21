namespace Domain.Surveillance.Scheduling
{
    using System;

    using Domain.Surveillance.Scheduling.Interfaces;

    using Newtonsoft.Json;

    public class ScheduledExecutionMessageBusSerialiser : IScheduledExecutionMessageBusSerialiser
    {
        private readonly IScheduleExecutionDtoMapper _mapper;

        public ScheduledExecutionMessageBusSerialiser(IScheduleExecutionDtoMapper mapper)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ScheduledExecution DeserialisedScheduledExecution(string serialisedExecution)
        {
            if (string.IsNullOrWhiteSpace(serialisedExecution)) return null;

            var deserialisedScheduledExecution =
                JsonConvert.DeserializeObject<RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution>(
                    serialisedExecution);
            var mappedDeserialisedExecution = this._mapper.MapToDomain(deserialisedScheduledExecution);

            return mappedDeserialisedExecution;
        }

        public string SerialiseScheduledExecution(ScheduledExecution execution)
        {
            var initialMapping = this._mapper.MapToDto(execution);

            return JsonConvert.SerializeObject(initialMapping);
        }
    }
}