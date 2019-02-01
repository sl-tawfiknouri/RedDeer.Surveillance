using Surveillance.Systems.DataLayer.Processes.Interfaces;

namespace Surveillance.Systems.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationThirdPartyDataRequestContext
    {
        string Id { get; }

        ISystemProcessOperationContext EndEvent();
        void EventError(string message);
        void StartEvent(ISystemProcessOperationThirdPartyDataRequest request);
    }
}