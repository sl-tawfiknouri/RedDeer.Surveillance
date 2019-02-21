using System;
using Domain.Scheduling.Interfaces;
using Newtonsoft.Json;

namespace Domain.Scheduling
{
    public class ScheduledExecutionMessageBusSerialiser : IScheduledExecutionMessageBusSerialiser
    {
        private readonly IScheduleExecutionDtoMapper _mapper;

        public ScheduledExecutionMessageBusSerialiser(IScheduleExecutionDtoMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public string SerialiseScheduledExecution(ScheduledExecution execution)
        {
            var initialMapping = _mapper.MapToDto(execution);
                
            return JsonConvert.SerializeObject(initialMapping);
        }

        public ScheduledExecution DeserialisedScheduledExecution(string serialisedExecution)
        {
            if (string.IsNullOrWhiteSpace(serialisedExecution))
            {
                return null;
            }

            var deserialisedScheduledExecution = JsonConvert.DeserializeObject<RedDeer.Contracts.SurveillanceService.Rules.ScheduledExecution>(serialisedExecution);
            var mappedDeserialisedExecution = _mapper.MapToDomain(deserialisedScheduledExecution);

            return mappedDeserialisedExecution;
        }
    }
}
