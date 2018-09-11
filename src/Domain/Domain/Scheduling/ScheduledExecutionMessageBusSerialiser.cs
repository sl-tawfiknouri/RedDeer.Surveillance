using Domain.Scheduling.Interfaces;
using Newtonsoft.Json;

namespace Domain.Scheduling
{
    public class ScheduledExecutionMessageBusSerialiser : IScheduledExecutionMessageBusSerialiser
    {
        public string SerialiseScheduledExecution(ScheduledExecution execution)
        {
            return JsonConvert.SerializeObject(execution);
        }

        public ScheduledExecution DeserialisedScheduledExecution(string serialisedExecution)
        {
            if (string.IsNullOrWhiteSpace(serialisedExecution))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<ScheduledExecution>(serialisedExecution);
        }
    }
}
