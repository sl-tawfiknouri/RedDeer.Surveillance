namespace Domain.Scheduling.Interfaces
{
    public interface IScheduledExecutionMessageBusSerialiser
    {
        ScheduledExecution DeserialisedScheduledExecution(string serialisedExecution);
        string SerialiseScheduledExecution(ScheduledExecution execution);
    }
}