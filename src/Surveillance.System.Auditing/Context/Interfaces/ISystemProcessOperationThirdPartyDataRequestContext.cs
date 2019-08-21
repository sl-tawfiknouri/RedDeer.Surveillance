namespace Surveillance.Auditing.Context.Interfaces
{
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationThirdPartyDataRequestContext
    {
        string Id { get; }

        ISystemProcessOperationContext EndEvent();

        void EventError(string message);

        void StartEvent(ISystemProcessOperationThirdPartyDataRequest request);
    }
}