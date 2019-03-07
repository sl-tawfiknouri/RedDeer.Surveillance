namespace Domain.Surveillance.Scheduling.Interfaces
{
    public interface IScheduledExecutionMessageBusSerialiser
    {
        ScheduledExecution DeserialisedScheduledExecution(string serialisedExecution);
        string SerialiseScheduledExecution(ScheduledExecution execution);
    }
}