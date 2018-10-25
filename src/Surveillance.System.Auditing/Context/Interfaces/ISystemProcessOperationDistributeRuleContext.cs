namespace Surveillance.System.Auditing.Context.Interfaces
{
    public interface ISystemProcessOperationDistributeRuleContext
    {
        ISystemProcessOperationContext EndEvent();
        void StartEvent(DataLayer.Processes.Interfaces.ISystemProcessOperationDistributeRule distributeRule);
        void EventError(string message);
    }
}