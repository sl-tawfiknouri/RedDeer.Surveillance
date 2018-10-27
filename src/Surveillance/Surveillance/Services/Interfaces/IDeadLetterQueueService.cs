namespace Surveillance.Services.Interfaces
{
    public interface IDeadLetterQueueService
    {
        void Initialise();
        void Terminate();
    }
}