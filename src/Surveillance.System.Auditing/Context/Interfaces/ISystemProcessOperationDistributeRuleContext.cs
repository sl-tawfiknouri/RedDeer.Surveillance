namespace Surveillance.Auditing.Context.Interfaces
{
    using Surveillance.Auditing.DataLayer.Processes.Interfaces;

    public interface ISystemProcessOperationDistributeRuleContext
    {
        string Id { get; }

        ISystemProcessOperationContext EndEvent();

        void EventError(string message);

        void StartEvent(ISystemProcessOperationDistributeRule distributeRule);
    }
}